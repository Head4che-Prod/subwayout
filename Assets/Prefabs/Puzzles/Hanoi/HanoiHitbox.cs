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
        [CanBeNull] public HanoiPiece containedBall;
        
        
        private void Start()
        {
            HanoiTowers.Instance.ColliderGrid[bar, height] = this;
        }
        
        /// <summary>
        /// Removes a ball from the grid.
        /// </summary>
        /// <param name="ball"><see cref="HanoiBall"/> to remove from the grid.</param>
        public static void RemoveBall(HanoiPiece ball)
        {
            foreach (HanoiHitbox hitbox in HanoiTowers.Instance.ColliderGrid)
                if (hitbox.containedBall == ball)
                    hitbox.containedBall = null;
        }
        
        private void OnTriggerEnter(Collider collisionInfo)
        {
            if (collisionInfo.TryGetComponent<HanoiPieceBall>(out HanoiPieceBall ball))
                HanoiTowers.Instance.RepositionBall(ball.parent, this);
            // Debug.Log($"{name} detected collision with {collisionInfo.gameObject.name}");
        }
    }
}