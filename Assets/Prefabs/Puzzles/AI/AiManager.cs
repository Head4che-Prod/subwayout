using System.Collections;
using Objects;
using UnityEngine;
using UnityEngine.AI;
using Prefabs.Player;
using Random = UnityEngine.Random;

public class AiManager : MonoBehaviour
{
    private static readonly int whichAnim = Animator.StringToHash("whichAnim");
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] GameObject cheese;
    private ObjectGrabbable _cheeseGrabbable;
    private Animator _animator;
    

    void Start()
    {
        _cheeseGrabbable = cheese.GetComponent<ObjectGrabbable>();
        _animator = GetComponent<Animator>();
        _animator.SetInteger(whichAnim, 0);
        
    }

    private void Update()
    {
        if (PlayerInteract.LocalPlayerInteract.GrabbedObject == _cheeseGrabbable)
        {
            MoveForwardTarget();
        }
        else //will flee 
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
            }
        }
    }
    /// <summary>
    /// Rat moves towards the grabbed cheese.
    /// </summary>
    private void MoveForwardTarget()
    {
        if (Vector3.Distance(_agent.transform.position, cheese.transform.position) < 2f)
        {
            _animator.SetInteger(whichAnim, 0);
            _agent.destination = transform.position;
        }
        else
        {
            _animator.SetInteger(whichAnim, -1);
            _agent.speed = 3f;
            _agent.SetDestination(cheese.transform.position);
            transform.LookAt(new Vector3(cheese.transform.position.x, cheese.transform.position.y, cheese.transform.position.z)); //face the target
        }
    }
    
    /// <summary>
    /// Rat goes to a valid computed random position.
    /// </summary>
    private void MoveRandom()
    {
        Vector3 randomPosition = transform.position + new Vector3(Random.Range(-2f,2f), 0f, Random.Range(-2f, 2f)); ;
        if (NavMesh.SamplePosition(randomPosition, out _, 2f, NavMesh.AllAreas)) //find if the randomPos is valid on the navmesh
        {
            _animator.SetInteger(whichAnim, -1);
            _agent.SetDestination(randomPosition);
        }
    }

    /// <summary>
    /// Makes the rat waits in a iddle state for a specific amount of time.
    /// </summary>
    private IEnumerator WaitStateEnumerator()
    {
        yield return new WaitForSeconds(2.5f);
    }
    
    
}
