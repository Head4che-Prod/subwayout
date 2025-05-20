using Prefabs.Player;
using Prefabs.Player.PlayerUI.DebugConsole;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace Prefabs.FreeCam
{
    public class ToggleableFreeCam : FreeCamera
    {
        private PlayerObject _player;
        private InputAction _exitAction;
        private void Start()
        {
            _player = PlayerObject.LocalPlayer;
            
            _exitAction = _player.Input.actions.FindAction("Gameplay/Pause");
            _exitAction.performed += DisableFreeCam;
            
            transform.position = _player.playerCamera.transform.position;
            transform.rotation = _player.playerCamera.transform.rotation;
            
            _player.Movement.enabled = false;
            _player.Interaction.enabled = false;
            _player.cameraController.enabled = false;
            DebugConsole.Singleton.ToggleConsole(new InputAction.CallbackContext());
            _player.transform.Find("Canvas").GetChild(1).gameObject.SetActive(false);
            
            _player.playerCamera.enabled = false;
            Debug.Log(_player.Input.currentActionMap.name);
        }

        private void DisableFreeCam(InputAction.CallbackContext _) => Destroy(this);
        
        private void OnDestroy()
        {
            _exitAction.performed -= DisableFreeCam;
            
            _player = PlayerObject.LocalPlayer;
            transform.position = _player.playerCamera.transform.position;
            transform.rotation = _player.playerCamera.transform.rotation;
            
            
            _player.Movement.enabled = true;
            _player.Interaction.enabled = true;
            _player.cameraController.enabled = true;
            DebugConsole.Singleton.ToggleConsole(new InputAction.CallbackContext());
            _player.transform.Find("Canvas").GetChild(1).gameObject.SetActive(PlayerObject.DisplayHints);
            
            _player.playerCamera.enabled = true;
        }
    }
}
