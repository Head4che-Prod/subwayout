using System;
using System.Runtime.Serialization;
using Objects;
using UnityEngine;
using UnityEngine.AI;
using Prefabs.Player;

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
            }
        }
    }

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
    
}
