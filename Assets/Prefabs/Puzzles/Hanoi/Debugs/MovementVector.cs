using UnityEngine;

namespace Prefabs.Puzzles.Hanoi.Debugs
{
    public class MovementVector : MonoBehaviour
    {
        public static LineRenderer Instance;

        private void Start()
        {
            Instance = GetComponent<LineRenderer>();
        }
    }
}