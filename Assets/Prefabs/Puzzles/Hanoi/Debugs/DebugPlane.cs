using UnityEngine;

namespace Prefabs.Puzzles.Hanoi.Debugs
{
    public class DebugPlane : MonoBehaviour
    {
        public static DebugPlane Instance;

        private void Start()
        {
            Instance = this;
        }
    }
}
