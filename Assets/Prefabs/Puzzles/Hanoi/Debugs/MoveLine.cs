using UnityEngine;

namespace Prefabs.Puzzles.Hanoi.Debugs
{
    public class MoveLine : MonoBehaviour
    {
        public static LineRenderer Instance;

        private void Start()
        {
            Instance = GetComponent<LineRenderer>();
        }
    }
}
