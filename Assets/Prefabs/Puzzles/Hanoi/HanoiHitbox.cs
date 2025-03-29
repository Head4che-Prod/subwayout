using System;
using UnityEngine;
using UnityEngine.Events;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiHitbox : MonoBehaviour
    {
        [NonSerialized] public UnityEvent<GameObject> CollisionEnterEvent;

        private void Awake()
        {
            CollisionEnterEvent = new UnityEvent<GameObject>();
        }

        private void OnTriggerEnter(Collider collisionInfo)
        {
            if (collisionInfo.gameObject.TryGetComponent<HanoiPieceBall>(out HanoiPieceBall ball))
                CollisionEnterEvent?.Invoke(ball.parent);
            // Debug.Log($"{name} detected collision with {collisionInfo.gameObject.name}");
        }
    }
}