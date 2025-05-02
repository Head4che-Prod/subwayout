using System;
using System.Runtime.Serialization;
using Objects;
using UnityEngine;
using UnityEngine.AI;
using Prefabs.Player;

public class AiManager : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] GameObject cheese;
    private ObjectGrabbable _cheeseGrabbable;

    void Start()
    {
        _cheeseGrabbable = cheese.GetComponent<ObjectGrabbable>();
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
                Vector3 direction = (PlayerInteract.LocalPlayerInteract.transform.position - transform.position).normalized;
                _agent.speed = 5f;
                _agent.SetDestination(transform.position - (7f*direction)); //moves in the opposite direction
            }
        }
    }

    private void MoveForwardTarget()
    {
        if (Vector3.Distance(_agent.transform.position, cheese.transform.position) < 2f)
        {
            _agent.destination = transform.position;
        }
        else
        {
            _agent.speed = 3f;
            _agent.SetDestination(cheese.transform.position);
            transform.LookAt(new Vector3(cheese.transform.position.x, cheese.transform.position.y, cheese.transform.position.z)); //face the target
        }
    }
    
}
