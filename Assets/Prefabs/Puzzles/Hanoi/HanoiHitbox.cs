using System;
using UnityEngine;
using UnityEngine.Events;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiHitbox : MonoBehaviour
    {
        [NonSerialized] public UnityEvent<GameObject> CollisionEnterEvent;

        private void Start()
        {
            CollisionEnterEvent = new UnityEvent<GameObject>();
        }

        private void OnTriggerEnter(Collider collisionInfo)
        {
            CollisionEnterEvent?.Invoke(collisionInfo.gameObject);
            // Debug.Log($"{name} detected collision with {collisionInfo.gameObject.name}");
        }
    }
}