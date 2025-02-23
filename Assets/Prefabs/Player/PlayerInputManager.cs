using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        private static HashSet<string> _validInputModes = new HashSet<string>()
        {
            "Gameplay",
            "Ui"
        };

        private PlayerObject _player;

        public void Start()
        {
            _player = GetComponent<PlayerObject>();
        }
        
        public static void SetPlayerInputMap(PlayerObject playerObject){}
    }
}