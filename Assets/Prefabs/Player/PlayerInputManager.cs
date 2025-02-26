using System;
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
            "UI",
            "DebugConsole"
        };

        private PlayerObject _player;

        public void Start()
        {
            _player = GetComponent<PlayerObject>();
            SetPlayerInputMap("Gameplay");
        }

        public void SetPlayerInputMap(string inputMap)
        {
            if (_validInputModes.Contains(inputMap))
            {
                _player.Input.currentActionMap.Disable();
                _player.Input.SwitchCurrentActionMap(inputMap);
            }
            else 
                Debug.LogError($"Invalid input map: {inputMap}");
        }
    }
}