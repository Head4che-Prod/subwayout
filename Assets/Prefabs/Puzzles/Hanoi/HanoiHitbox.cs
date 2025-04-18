using JetBrains.Annotations;
using UnityEngine;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiHitbox : MonoBehaviour
    {
        /// <summary>
        /// Position of the collider on the grid's vertical axis.
        /// </summary>
        [SerializeField, Range(0,2)] public ushort height;
        /// <summary>
        /// Position of the collider on the grid's horizontal axis.
        /// </summary>
        [SerializeField, Range(0,2)] public ushort bar;
        
        /// <summary>
        /// Ball currently stored in the collider.
        /// </summary>
        [CanBeNull] public HanoiBall containedBall;
        
        
        private void Start()
        {
            HanoiTowers.Instance.ColliderGrid[bar, height] = this;
        }
        
        /// <summary>
        /// Removes a ball from the grid.
        /// </summary>
        /// <param name="ball"><see cref="HanoiBall"/> to remove from the grid.</param>
        public static void RemoveBall(HanoiBall ball)
        {
            foreach (HanoiHitbox hitbox in HanoiTowers.Instance.ColliderGrid)
                if (hitbox.containedBall == ball)
                    hitbox.containedBall = null;
        }
        
        /// <summary>
        /// Reset a ball object's position to its internal position.
        /// </summary>
        /// <param name="ballTransform"><see cref="Transform"/> of the ball whose position is reset.</param>
        public static void ResetBall(Transform ballTransform)
        {
            foreach (HanoiHitbox hitbox in HanoiTowers.Instance.ColliderGrid)
                if (ballTransform == hitbox.containedBall?.gameObject.transform)
                {
                    ballTransform.localPosition = new Vector3(
                        hitbox.gameObject.transform.localPosition.x,
                        0.0135f,
                        hitbox.gameObject.transform.localPosition.z
                    );
                }
        }
        
        private void OnTriggerEnter(Collider collisionInfo)
        {
            if (collisionInfo.TryGetComponent<HanoiPieceBall>(out HanoiPieceBall ball))
                HanoiTowers.Instance.RepositionBall(ball.parent, this);
            // Debug.Log($"{name} detected collision with {collisionInfo.gameObject.name}");
        }
    }
}