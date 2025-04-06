using UnityEngine;

namespace Prefabs.Puzzles.Hanoi
{        
    /// <summary>
    /// Internal representation of a hitbox, storing its position and content.
    /// </summary>
    public class HanoiCollider
    {
        /// <summary>
        /// Table representation internal state of the game.
        /// </summary>
        public static string DebugGrid()
        {
            return "<mspace=0.55em>|------------|------------|------------|\n" +
                   $"|{ColliderGrid[0, 2].ContainedBall?.Object.name,12}|{ColliderGrid[1, 2].ContainedBall?.Object.name,12}|{ColliderGrid[2, 2].ContainedBall?.Object.name,12}|\n" +
                   "|------------|------------|------------|\n" +
                   $"|{ColliderGrid[0, 1].ContainedBall?.Object.name,12}|{ColliderGrid[1, 1].ContainedBall?.Object.name,12}|{ColliderGrid[2, 1].ContainedBall?.Object.name,12}|\n" +
                   "|------------|------------|------------|\n" +
                   $"|{ColliderGrid[0, 0].ContainedBall?.Object.name,12}|{ColliderGrid[1, 0].ContainedBall?.Object.name,12}|{ColliderGrid[2, 0].ContainedBall?.Object.name,12}|\n" +
                   "|------------|------------|------------|</mspace>";
        }
        
        /// <summary>
        /// Internal grid the game uses to store the positions of the balls.
        /// </summary>
        public static readonly HanoiCollider[,] ColliderGrid = new HanoiCollider[3, 3];
        
        /// <summary>
        /// Position of the collider on the grid's vertical axis.
        /// </summary>
        public readonly ushort Height;
        /// <summary>
        /// Position of the collider on the grid's horizontal axis.
        /// </summary>
        public readonly ushort Bar;

        /// <summary>
        /// Object the collider is linked to.
        /// </summary>
        private GameObject Object { get; }
        /// <summary>
        /// Ball currently stored in the collider.
        /// </summary>
        public HanoiBall ContainedBall;
        
        
        /// <summary>
        /// HanoiCollider constructor.
        /// </summary>
        /// <param name="detector"><see cref="GameObject"/> the collider represents.</param>
        /// <param name="detectorHeight"><see cref="ushort"/> representing the position of the collider on the grid's vertical axis.</param>
        /// <param name="detectorBar"><see cref="ushort"/> representing the position of the collider on the grid's horizontal axis.</param>
        public HanoiCollider(GameObject detector, ushort detectorHeight, ushort detectorBar)
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
        
        /// <summary>
        /// Removes a ball from the grid.
        /// </summary>
        /// <param name="ball"><see cref="HanoiBall"/> to remove from the grid.</param>
        public static void RemoveBall(HanoiBall ball)
        {
            foreach (HanoiCollider collider in ColliderGrid)
                if (collider.ContainedBall == ball)
                    collider.ContainedBall = null;
        }
        
        /// <summary>
        /// Reset a ball object's position to its internal position.
        /// </summary>
        /// <param name="ballTransform"><see cref="Transform"/> of the ball whose position is reset.</param>
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
