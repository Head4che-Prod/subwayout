using System;
using HomeMenu;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
namespace Prefabs.Player.PlayerUI.PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        private PlayerObject _player;
        private InputAction _pauseAction;
        private InputAction _unpauseAction;

        private GameObject _pauseMenuUI;
        private Action<InputAction.CallbackContext> _pause;
        private Action<InputAction.CallbackContext> _unpause;

        [SerializeField] private DynamicButton[] buttons;

        void Awake()
        {
            if (buttons.Length == 0)
                Debug.LogError($"UI {name} has no buttons");

            _pauseMenuUI = transform.Find("PauseMenuUI").gameObject;
            _pauseMenuUI.SetActive(false);
        }

        void Start()
        {
            _player = GetComponentInParent<PlayerObject>();
            _pauseAction = _player.Input.actions["Pause"];
            _unpauseAction = _player.Input.actions["Cancel"];

            _pause = _ => Pause();
            _unpause = _ => Resume();
            _pauseAction.performed += _pause;
            _unpauseAction.performed += _unpause;
        }

        public void Resume()
        {
            _pauseMenuUI.SetActive(false);
            _player.InputManager.SetPlayerInputMap("Gameplay");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Pause()
        {
            _pauseMenuUI.SetActive(true);
            _player.InputManager.SetPlayerInputMap("UI");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


        public void QuitGame()
        {
            _pauseAction.performed -= _pause;
            _unpauseAction.performed -= _unpause;
            Destroy(NetworkManager.Singleton.gameObject);
            SceneManager.LoadScene("Scenes/HomeMenu", LoadSceneMode.Single);
            _ = SessionManager.Singleton.LeaveSession();
        }
    }
}