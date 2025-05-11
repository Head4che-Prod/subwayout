using System.Collections;
using Objects;
using Prefabs.Player;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Prefabs.Puzzles.AI
{
    public class AiManager : MonoBehaviour
    {
        private static readonly int whichAnim = Animator.StringToHash("whichAnim");
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private GameObject cheeseInCage;
        private Animator _animator;
        [SerializeField] private GameObject keyRat;
    

        void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.SetInteger(whichAnim, 0);
        }

        private void Update()
        {
            if (Vector3.Distance(_agent.transform.position, cheeseInCage.gameObject.transform.position) < 1f)
            {
                keyRat.SetActive(false);
                GameObject spawnedObj = Instantiate(keyRat, cheeseInCage.transform.position + new Vector3(2, 0, 0),
                    Quaternion.identity);
                spawnedObj.transform.localScale = keyRat.transform.localScale;
                spawnedObj.SetActive(true);

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
            if (Vector3.Distance(_agent.transform.position, cheeseInCage.transform.position) < 2f)
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
