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
        
        
        //if ()
        //{
         //   MoveForwardTarget(PlayerInteract.LocalPlayerInteract.gameObject);
        //}
    }

    private void MoveForwardTarget()
    {
        if (Vector3.Distance(_agent.transform.position, cheese.transform.position) < 2f)
        {
            _agent.destination = transform.position;
        }
        else
        {
            _agent.SetDestination(cheese.transform.position);
            transform.LookAt(new Vector3(cheese.transform.position.x, cheese.transform.position.y, cheese.transform.position.z)); //face the target
        }
    }
}
