using System;
using UnityEngine;
using UnityEngine.Events;

public class HanoiHitbox : MonoBehaviour
{
    [NonSerialized] public UnityEvent<GameObject> CollisionEnterEvent;

    private void Start()
    {
        CollisionEnterEvent = new UnityEvent<GameObject>();
    }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        CollisionEnterEvent?.Invoke(collisionInfo.gameObject);
        Debug.Log("Something happened");
    }
}