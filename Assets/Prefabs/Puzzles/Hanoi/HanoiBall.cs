using UnityEngine;

namespace Prefabs.Puzzles.Hanoi
{
    /// <summary>
    /// Internal representation of a ball, storing its size and position.
    /// </summary>
    public class HanoiBall : MonoBehaviour
    {
        /// <summary>
        /// Weight of the ball. Balls can not be placed on heavier ones.
        /// </summary>}
        [SerializeField, Range(0, 2)] public int weight;
        
        private void Start()
        {
            gameObject.transform.localPosition = new Vector3(0.12f, 0.0135f, 0.033f + 0.03f * weight);
        }
        
    }
}