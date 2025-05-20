using System.Collections.Generic;
using UnityEngine;

namespace Prefabs.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        private static readonly HashSet<string> ValidInputModes = new HashSet<string>()
        {
            "Gameplay",
            "UI",
            "DebugConsole"
        };

        private PlayerObject _player;

        public void Start()
        {
            _player = GetComponent<PlayerObject>();
            foreach (string map in ValidInputModes)
                _player.Input.actions.FindActionMap(map).Disable();
            SetPlayerInputMap("Gameplay");
        }

        public void SetPlayerInputMap(string inputMap)
        {
            if (ValidInputModes.Contains(inputMap))
            {
                _player.Input.currentActionMap.Disable();
                _player.Input.SwitchCurrentActionMap(inputMap);
            }
            else 
                Debug.LogError($"Invalid input map: {inputMap}");
        }
    }
}