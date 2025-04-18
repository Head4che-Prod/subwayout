using UnityEngine;
using UnityEngine.Serialization;

namespace Prefabs.Puzzles.Hanoi
{
    /// <summary>
    /// End part of a Hanoi piece that stays inside the box. Used to detect if the object colliding with HanoiCollider is a ball.
    /// </summary>
    public class HanoiPieceBall : MonoBehaviour
    {
        /// <summary>
        /// The piece the HanoiPieceBall is part of.
        /// </summary>
        public HanoiBall parent;
    }
}
