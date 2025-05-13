using System.Collections;
using System.Collections.Generic;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Prefabs.Puzzles.AI
{
    public class AiManager : NetworkBehaviour
    {
        private static readonly int WhichAnim = Animator.StringToHash("whichAnim");
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private GameObject cheeseInCage;
        private Animator _animator;
        [SerializeField] private GameObject keyModelInMouthRat;
        [SerializeField] private GameObject keyGrabbable;
        [SerializeField] private GameObject ClonedRat;
        private Animator _clonedRatAnimator;
        [SerializeField] private CageManager _cageManager;
        private Animator _cageAnimator;
        private readonly GameObject[] _playersArray = GameObject.FindGameObjectsWithTag("Player");


        private enum State
        {
            Idle,
            Fleeing,
            Baited
        }

        private State _state;

        void Start()
        {
            if (!IsServer)
            {
                return;
            }
            _animator = GetComponent<Animator>();
            _animator.SetInteger(WhichAnim, 0);
            _clonedRatAnimator = ClonedRat.GetComponent<Animator>();
            _cageAnimator = _cageManager.gameObject.GetComponentInChildren<Animator>();
            _state = State.Idle;
            StartCoroutine(IdleCoroutine());
        }

        private void Update()
        {
            // Rat enters cage
            if (Vector3.Distance(_agent.transform.position, cheeseInCage.gameObject.transform.position) < 0.5f &&
                cheeseInCage.gameObject.activeInHierarchy)
            {
                keyModelInMouthRat.SetActive(false);
                GameObject spawnedObj = Instantiate(keyGrabbable,
                    cheeseInCage.transform.position + new Vector3(2, 0.25f, 0),
                    transform.rotation);
                spawnedObj.GetComponent<NetworkObject>()
                    .Spawn(); //only done once so okay for expensive method invocation
                spawnedObj.SetActive(true);
                this.gameObject.SetActive(false);
                ClonedRat.SetActive(true);
                _clonedRatAnimator.Play("Idle");
                _cageAnimator.SetBool("animCageDoor", !_cageAnimator.GetBool("animCageDoor"));
                return;
            }

            // Rat goes towards cage
            if (cheeseInCage.gameObject.activeSelf && Vector3.Distance(_agent.transform.position,
                    FindNearestPlayer().transform.position) > 5f)
            {
                MoveTowardsCheeseInCage();
            }
            else // Rat will flee or wait
            {
                if (Vector3.Distance(_agent.transform.position, FindNearestPlayer().transform.position) <
                    5f)
                {
                    Flee();
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
        
        /// <summary>
        /// Makes rat flee from the player.
        /// </summary>

        private void Flee()
        {
            _state = State.Fleeing;
            Vector3 dirAway = (transform.position - FindNearestPlayer().transform.position)
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
             if (Mathf.Approximately(_agent.transform.position.x, cheeseInCage.transform.position.x) && Mathf.Approximately(_agent.transform.position.z, cheeseInCage.transform.position.z) ) // approximately calculate if they're equal
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
                _animator.SetInteger(WhichAnim, 0);
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

        private GameObject FindNearestPlayer()
        {
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