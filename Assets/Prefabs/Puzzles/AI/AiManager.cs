using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Prefabs.Puzzles.AI
{
    public class AiManager : NetworkBehaviour
    {
        [SerializeField] private GameObject cheeseInCage;
        [SerializeField] private GameObject keyModelInMouthRat;
        [SerializeField] private GameObject keyGrabbable;
        [SerializeField] private GameObject clonedRat;
        [SerializeField] private CageManager cageManager;
        
        private NavMeshAgent _agent;
        private GameObject[] _playersArray;
        
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
            if (!IsServer)
            {
                enabled = false;
                return;
            }
            
            _agent = GetComponent<NavMeshAgent>();
            _playersArray = GameObject.FindGameObjectsWithTag("Player");
            _animator = GetComponent<Animator>();

            _animator.SetInteger(WhichAnim, 0);
            _clonedRatAnimator = clonedRat.GetComponent<Animator>();
            _cageAnimator = cageManager.gameObject.GetComponentInChildren<Animator>();
            _state = State.Idle;

            StartCoroutine(IdleCoroutine());
        }

        private void Update()
        {
            GameObject nearestPlayer = FindNearestPlayer();

            // Rat enters cage
            if (Vector3.Distance(_agent.transform.position, cheeseInCage.gameObject.transform.position) < 0.5f &&
                cheeseInCage.gameObject.activeInHierarchy)
            {
                GameObject spawnedObj = Instantiate(keyGrabbable,
                    cheeseInCage.transform.position + new Vector3(2, 0.25f, 0),
                    transform.rotation);
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation - only done once so okay for expensive method invocation
                spawnedObj.GetComponent<NetworkObject>().Spawn();
                spawnedObj.SetActive(true);
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

        [Rpc(SendTo.Everyone)]
        private void DisableRatRpc()
        {
            keyModelInMouthRat.SetActive(false);
            this.gameObject.SetActive(false);

            clonedRat.SetActive(true);
            _clonedRatAnimator.Play("Idle");
            _cageAnimator.SetBool("animCageDoor", !_cageAnimator.GetBool("animCageDoor"));
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
                    _animator.SetInteger(WhichAnim, 1);
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
                _animator.SetInteger(WhichAnim, 0);
            }
            else
            {
                _animator.SetInteger(WhichAnim, -1);
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

                    _animator.SetInteger(WhichAnim, -1);
                    _agent.speed = 1.5f;
                    _agent.SetDestination(hit.position);
                    yield return new WaitUntil(() => transform.position - hit.position == Vector3.zero);
                }
            }
        }

        /// <summary>
        /// Keep animating until the target destination changes or is reached.
        /// </summary>
        private IEnumerator AnimateUntilDestinationCoroutine()
        {
            Vector3 target = _agent.destination;
            while (_agent.destination == target && transform.position != target)
                yield return null;
            _animator.SetInteger(WhichAnim, (int)_state);
        }
        
        /// <summary>
        /// Finds the nearest player and returns its gameObject
        /// </summary>
        /// <returns>The nearest player's <see cref="GameObject"/>.</returns>
        private GameObject FindNearestPlayer()
        {
            if (_playersArray.Length != NetworkManager.Singleton.ConnectedClients.Count)
            {
                _playersArray = GameObject.FindGameObjectsWithTag("Player");
            }

            float closestDist = Mathf.Infinity;
            GameObject closestPlayer = null;
            foreach (GameObject player in _playersArray)
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
    }
}