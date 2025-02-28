using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIStateWandering : MonoBehaviour
{
    private bool _waiting = false;

    [SerializeField] private float range = 5;
    [SerializeField] private float minWaitTime = 0;
    [SerializeField] private float maxWaitTime = 2;

    private AIMovement _aiMovement;

    private void Awake()
    {
        _aiMovement = GetComponent<AIMovement>();
    }

    public void StateUpdate()
    {
        if (_waiting) return;

        _waiting = true;
        ;
        _aiMovement.MoveToRandom(transform.position, range);
        StartCoroutine(StopWait());
    }

    private IEnumerator StopWait()
    {
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        _waiting = false;
        
    }
}
