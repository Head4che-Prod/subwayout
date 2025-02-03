using System;
using UnityEngine;
using UnityEngine.Events;

public class HanoiHitbox : MonoBehaviour
{
    [NonSerialized] public UnityEvent<GameObject> CollisionEnterEvent;

    private void Awake()
    {
        CollisionEnterEvent = new UnityEvent<GameObject>();
    }

    private void OnTriggerEnter(Collider collisionInfo)
    {
        CollisionEnterEvent?.Invoke(collisionInfo.gameObject);
        // Debug.Log($"{name} detected collision with {collisionInfo.gameObject.name}");
    }
}