using UnityEngine;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiCollider
    {
        public static string DebugGrid()
        {
            return "|-------------|-------------|-------------|\n" +
                   $"|{ColliderGrid[0, 2].ContainedBall?.Object.name,13}|{ColliderGrid[1, 2].ContainedBall?.Object.name,13}|{ColliderGrid[2, 2].ContainedBall?.Object.name,13}|\n" +
                   "|-------------|-------------|-------------|\n" +
                   $"|{ColliderGrid[0, 1].ContainedBall?.Object.name,13}|{ColliderGrid[1, 1].ContainedBall?.Object.name,13}|{ColliderGrid[2, 1].ContainedBall?.Object.name,13}|\n" +
                   "|-------------|-------------|-------------|\n" +
                   $"|{ColliderGrid[0, 0].ContainedBall?.Object.name,13}|{ColliderGrid[1, 0].ContainedBall?.Object.name,13}|{ColliderGrid[2, 0].ContainedBall?.Object.name,13}|\n" +
                   "|-------------|-------------|-------------|";
        }
        
        
        public static readonly HanoiCollider[,] ColliderGrid = new HanoiCollider[3, 3];
        public readonly int Height;
        public readonly int Bar;

        private GameObject Object { get; }
        public HanoiBall ContainedBall;
        
        public HanoiCollider(GameObject detector, int detectorHeight, int detectorBar)
        {
            Object = detector;
            Height = detectorHeight;
            Bar = detectorBar;
            ColliderGrid[Bar, Height] = this;

            HanoiHitbox hitbox = detector.GetComponent<HanoiHitbox>();

            hitbox.CollisionEnterEvent.AddListener(OnCollisionEnter);
        }

        private void OnCollisionEnter(GameObject other)
        {
            // Debug.Log($"{Object.name} heard about collision with {other.name}");
            HanoiTowers.Instance.BallEnterBoxEvent?.Invoke(other, this);    
        }

        public static void RemoveBall(HanoiBall ball)
        {
            foreach (HanoiCollider collider in ColliderGrid)
                if (collider.ContainedBall == ball)
                    collider.ContainedBall = null;
        }

        public static void ResetBall(Transform ballTransform)
        {
            foreach (HanoiCollider collider in ColliderGrid)
                if (ballTransform == collider.ContainedBall?.Object.transform)
                {
                    ballTransform.localPosition = new Vector3(
                        collider.Object.transform.localPosition.x,
                        0.0135f,
                        collider.Object.transform.localPosition.z
                    );
                }
        }
    }
}
