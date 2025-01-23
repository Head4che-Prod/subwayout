using System;
using UnityEngine;
using UnityEngine.Events;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiCollider
    {
        public static readonly HanoiCollider[,] ColliderGrid = new HanoiCollider[3, 3];
        public readonly int Height;
        public readonly int Bar;

        public GameObject Object { get; }
        public HanoiBall ContainedBall;
        
        private readonly UnityEvent<GameObject, HanoiCollider> _ballEnterBoxEvent;
        
        public HanoiCollider(GameObject detector, int detectorHeight, int detectorBar,
            UnityEvent<GameObject, HanoiCollider> ballEnterBoxEvent)
        {
            Object = detector;
            Height = detectorHeight;
            Bar = detectorBar;
            ColliderGrid[Bar, Height] = this;

            HanoiHitbox hitbox = detector.GetComponent<HanoiHitbox>();

            _ballEnterBoxEvent = ballEnterBoxEvent;
            hitbox.CollisionEnterEvent.AddListener(OnCollisionEnter);
        }

        private void OnCollisionEnter(GameObject other)
        {
            // Debug.Log($"{Object.name} heard about collision with {other.name}");
            _ballEnterBoxEvent?.Invoke(other, this);    
        }

        public static void RemoveBall(HanoiBall ball)
        {
            foreach (HanoiCollider collider in ColliderGrid)
                if (collider.ContainedBall == ball)
                    collider.ContainedBall = null;
        }

        public static void ResetBall(Rigidbody ballBody)
        {
            foreach (HanoiCollider collider in ColliderGrid)
                if (ballBody == collider.ContainedBall?.Body)
                {
                    Debug.Log($"Let go of ball at ({ballBody.position.x}, {ballBody.position.y}, {ballBody.position.z})");
                    ballBody.position = new Vector3(
                        2.5f,
                        collider.Object.transform.position.y,
                        0.9f
                    );
                }
        }
    }
}
