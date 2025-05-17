using System.Collections;
using System.Linq;
using Hints;
using Objects;
using Prefabs.GameManagers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Prefabs.Puzzles.AI
{
    public class AiManager : NetworkBehaviour, IResettablePosition
    {
        [SerializeField] private GameObject cheeseInCage;
        [SerializeField] private GameObject keyModelInMouthRat;
        [SerializeField] private NetworkObject keyGrabbable;
        [SerializeField] private GameObject clonedRat;
        [SerializeField] private CageManager cageManager;
        
        public Vector3 InitialPosition { get; set; }
        public Quaternion InitialRotation { get; set; }
        
        private NavMeshAgent _agent;
        private GameObject[] _players;
        
        private static readonly int WhichAnim = Animator.StringToHash("whichAnim");
        private Animator _animator;
        private Animator _clonedRatAnimator;
        private Animator _cageAnimator;

        /// <summary>
        /// In what state the rat's AI is in. <br/>
        /// The integers correspond to the animation to default to when pathfinding finished.
        /// </summary>
        private enum State
        {
            Idle = 0,
            Fleeing = 1,
            Baited = -1
        }
        
        private State _state;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            AnimeRatRpc(0);
            _clonedRatAnimator = clonedRat.GetComponent<Animator>();
            _cageAnimator = cageManager.gameObject.GetComponentInChildren<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _players = FindPlayers();
            _state = State.Idle;
            InitialPosition = transform.position;
            InitialRotation = transform.rotation;
            ObjectPositionManager.Singleton.ResettableObjects.Add(this);
            
            StartCoroutine(IdleCoroutine());
        }

        private void Update()
        {
            if (!IsServer)
            {
                return;
            }
            
            GameObject nearestPlayer = FindNearestPlayer();

            // Rat enters cage
            if (Vector3.Distance(_agent.transform.position, cheeseInCage.gameObject.transform.position) < 0.5f &&
                cheeseInCage.gameObject.activeInHierarchy)
            {
                NetworkObject spawnedObj = Instantiate(keyGrabbable,
                    transform.position,
                    transform.rotation);
                spawnedObj.Spawn();
                spawnedObj.gameObject.SetActive(true);
                ObjectHighlightManager.RegisterHighlightableObject(spawnedObj.NetworkObjectId);
                
                DisableRatRpc();
                return;
            }

            // Rat goes towards cage
            if (nearestPlayer is not null && cheeseInCage.gameObject.activeSelf && Vector3.Distance(
                    _agent.transform.position,
                    nearestPlayer.transform.position) > 5f)
            {
                _state = State.Baited;
                MoveTowardsCheeseInCage();
            }
            else // Rat will flee or wait
            {
                if (nearestPlayer is not null &&
                    Vector3.Distance(_agent.transform.position, nearestPlayer.transform.position) <
                    5f)
                {
                    Flee(nearestPlayer);
                }
                else
                {
                    if (_state != State.Idle)
                    {
                        _state = State.Idle;
                        StartCoroutine(IdleCoroutine());
                    }
                }
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void DisableRatRpc()
        {
            keyModelInMouthRat.SetActive(false);
            gameObject.SetActive(false);

            clonedRat.SetActive(true);
            _clonedRatAnimator.Play("Idle");
            cageManager.ChangeCageDoorServerRpc(!_cageAnimator.GetBool("animCageDoor"));
                        
            HintSystem.EnableHints(Hint.RatKey);
            HintSystem.DisableHints(Hint.RatTrap);
        }


        /// <summary>
        /// Makes rat flee from the player.
        /// </summary>
        private void Flee(GameObject nearestPlayer)
        {
            _state = State.Fleeing;
            Vector3 dirAway = (transform.position - nearestPlayer.transform.position)
                .normalized;
            dirAway = Quaternion.AngleAxis(Random.Range(-90, 90), Vector3.up) *
                      dirAway; // we add a random angle because else it gets stuck at the end of the navmesh instead of turning 
            Vector3 finalDirection = transform.position + dirAway * 5f;
            if (NavMesh.SamplePosition(finalDirection, out NavMeshHit hit, 2.0f,
                    NavMesh.AllAreas)) //check if it's on the navmesh
            {
                if (Vector3.Distance(_agent.destination, hit.position) > 0.5f)
                {
                    AnimeRatRpc(1);
                    _agent.speed = 5f;
                    _agent.SetDestination(hit.position);
                }
            }
        }

        /// <summary>
        /// Moves the rat towards the cheese in the cage.
        /// </summary>
        private void MoveTowardsCheeseInCage()
        {
            //If just under the cage but can't reach it
            if (Mathf.Approximately(_agent.transform.position.x, cheeseInCage.transform.position.x) &&
                Mathf.Approximately(_agent.transform.position.z,
                    cheeseInCage.transform.position.z)) // approximately calculate if they're equal
            {
                AnimeRatRpc(0);
            }
            else
            {
                AnimeRatRpc(-1);
                _agent.speed = 3f;
                _agent.SetDestination(cheeseInCage.transform.position);
            }
        }

        /// <summary>
        /// Coroutine that manages the rat's movement in idle state.
        /// </summary>
        private IEnumerator IdleCoroutine()
        {
            while (_state == State.Idle) // continue while we are in an idle state
            {
                StartCoroutine(AnimateUntilDestinationCoroutine());
                yield return new WaitForSeconds(5);

                if (_state is State.Idle) // check if we are still in an idle state
                {
                    Vector3 dirAway = new Vector3(Random.Range(-1.5f, 1.5f), 0f, Random.Range(-1.5f, 1.5f));

                    NavMeshHit hit;
                    while (!NavMesh.SamplePosition(dirAway, out hit, 2.0f, NavMesh.AllAreas))
                    {
                        yield return null; // wait 1 frame
                    } //check if it's on the navmesh and if not will wait 1 frame before valid pos

                    AnimeRatRpc(-1);
                    _agent.speed = 1.5f;
                    _agent.SetDestination(hit.position);
                    yield return new WaitUntil(() => transform.position - hit.position == Vector3.zero);
                }
            }
        }
        
        
        /// <summary>
        /// Will play the corresponding animation of the rat thanks to the int parameter.
        /// </summary>
        /// <param name="intofAnimation"> int corresponding to a specific animation. 0 : idle  1 : fleeing -1 : baited </param>
        [Rpc(SendTo.ClientsAndHost)]
        private void AnimeRatRpc(int intofAnimation)
        {
            _animator.SetInteger(WhichAnim, intofAnimation);
        }
        
        

        /// <summary>
        /// Keep animating until the target destination changes or is reached.
        /// </summary>
        private IEnumerator AnimateUntilDestinationCoroutine()
        {
            Vector3 target = _agent.destination;
            while (_agent.destination == target && transform.position != target)
                yield return null;
            AnimeRatRpc((int)_state);
        }

        private GameObject[] FindPlayers() =>
            NetworkManager.ConnectedClientsList.Select(client => client.PlayerObject.gameObject).ToArray();
        
        /// <summary>
        /// Finds the nearest player and returns its gameObject
        /// </summary>
        /// <returns>The nearest player's <see cref="GameObject"/>.</returns>
        private GameObject FindNearestPlayer()
        {
            if (_players.Length != NetworkManager.Singleton.ConnectedClients.Count)
            {
                _players = FindPlayers();
            }

            float closestDist = Mathf.Infinity;

            GameObject closestPlayer = null;
            foreach (GameObject player in _players)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < closestDist)
                {
                    closestDist = distance;
                    closestPlayer = player;
                }
            }

            return closestPlayer;
        }

        public void ResetPosition() => ResetPositionClientRpc();

        [Rpc(SendTo.ClientsAndHost)]
        private void ResetPositionClientRpc()
        {
            transform.position = InitialPosition;
            transform.rotation = InitialRotation;
            if (IsServer)
                _agent.SetDestination(InitialPosition);
        }
    }
}