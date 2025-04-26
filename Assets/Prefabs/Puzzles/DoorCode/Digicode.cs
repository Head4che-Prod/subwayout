using UnityEngine;

namespace Prefabs.Puzzles.DoorCode
{
    public class Digicode : MonoBehaviour
    {
        private const int Password = 7642;
        private static int _enteredPassword = 0;
        /// <summary>
        /// Whether the current code is valid.
        /// </summary>
        public static bool CanDoorOpen => _enteredPassword == Password; 
        /// <summary>
        /// Whether the puzzle able to be interacted with.
        /// </summary>
        public static bool Active = true;
        public void Start()
        {
            Active = true;
            _enteredPassword = 0;
            int[] ordersOfMagnitude = { 1000, 100, 10, 1 };
            for (int i = 0; i < 4; i++)
            {
                Tile tile = transform.GetChild(i).GetComponent<Tile>();
                tile.Order = ordersOfMagnitude[i];
                tile.value.OnValueChanged += (oldVal, newVal) =>
                {
                    _enteredPassword += (newVal - oldVal) * tile.Order;
                };
            }
        }
    }
}
