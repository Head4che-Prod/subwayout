using UnityEngine;

namespace Prefabs.Puzzles.Hanoi.Debugs
{
    public class GrabPointVisualizer : MonoBehaviour
    {
        public static GrabPointVisualizer Instance;

        private void Start()
        {
            Instance = this;
        }
    }
}