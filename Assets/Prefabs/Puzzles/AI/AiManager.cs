using System.Collections;
using Objects;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Prefabs.Puzzles.AI
{
    public class AiManager : MonoBehaviour
    {
        private static readonly int whichAnim = Animator.StringToHash("whichAnim");
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private GameObject cheeseInCage;
        private Animator _animator;
        [SerializeField] private GameObject keyModelInMouthRat;
        [SerializeField] private GameObject keyGrabbable;
        [SerializeField] private GameObject ClonedRat;
        private Animator _clonedRatAnimator;
        [SerializeField] private CageManager _cageManager;
        private Animator _cageAnimator;
    

        void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.SetInteger(whichAnim, 0);
            _clonedRatAnimator = ClonedRat.GetComponent<Animator>();
            _cageAnimator = _cageManager.gameObject.GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (Vector3.Distance(_agent.transform.position, cheeseInCage.gameObject.transform.position) < 1f && cheeseInCage.gameObject.activeInHierarchy)
            {
                keyModelInMouthRat.SetActive(false);
                GameObject spawnedObj = Instantiate(keyGrabbable, cheeseInCage.transform.position + new Vector3(2, 0.25f, 0),
                    transform.rotation);
                spawnedObj.GetComponent<NetworkObject>().Spawn(); //only done once so okay for expensive method invocation
                spawnedObj.SetActive(true);
                this.gameObject.SetActive(false);
                ClonedRat.SetActive(true);
                _clonedRatAnimator.Play("Idle");
                _cageAnimator.SetBool("animCageDoor", !_cageAnimator.GetBool("animCageDoor"));
                return;
            }
            
            if(cheeseInCage.gameObject.activeSelf && Vector3.Distance(_agent.transform.position, PlayerInteract.LocalPlayerInteract.transform.position)> 5f)
            {
                _animator.SetInteger(whichAnim, -1);
                MoveForwardTarget();
            }
            else //will flee or wait
            {
                if (Vector3.Distance(_agent.transform.position, PlayerInteract.LocalPlayerInteract.transform.position) < 5f)
                {
                    _animator.SetInteger(whichAnim, 1);
                    Vector3 direction = (PlayerInteract.LocalPlayerInteract.transform.position - transform.position).normalized;
                    _agent.speed = 5f;
                    _agent.SetDestination(transform.position - (7f*direction)); //moves in the opposite direction
                }
                else
                {
                    _animator.SetInteger(whichAnim, 0);
                    StartCoroutine(WaitStateEnumerator());
                    //MoveRandom();
                }
            }
        }
        /// <summary>
        /// Rat moves towards the cheese in cage.
        /// </summary>
        private void MoveForwardTarget()
        {
            if (Vector3.Distance(_agent.transform.position, cheeseInCage.transform.position)== 0f)
            {
                _animator.SetInteger(whichAnim, 0);
                _agent.destination = transform.position;
            }
            else
            {
                _agent.speed = 3f;
                _agent.SetDestination(cheeseInCage.transform.position);
                transform.LookAt(new Vector3(cheeseInCage.transform.position.x, 0, cheeseInCage.transform.position.z)); //face the target but not on the y axis
            }
        }
    
        /// <summary>
        /// Rat goes to a valid computed random position.
        /// </summary>
        private void MoveRandom()
        {
            Vector3 randomPosition = transform.position + new Vector3(Random.Range(-10f,10f), 0f, Random.Range(-10f, 10f)); ;
            if (NavMesh.SamplePosition(randomPosition, out _, 10f, NavMesh.AllAreas)) //find if the randomPos is valid on the navmesh
            {
                _animator.SetInteger(whichAnim, -1);
                _agent.SetDestination(randomPosition);
            }
        }

        /// <summary>
        /// Makes the rat waits in an iddle state for a specific amount of time.
        /// </summary>
        private IEnumerator WaitStateEnumerator()
        {
            yield return new WaitForSeconds(10f);
        }
    
    
    }
}
