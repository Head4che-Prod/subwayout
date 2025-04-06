using HomeMenu;
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

        private static bool _allowMenuChange;      // Prevents multi-trigger
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
            _pauseAction.performed += _ => Pause();
            _unpauseAction.performed += _ => Resume();
            _allowMenuChange = true;
        }

        public void Resume()
        {
            _allowMenuChange = true;

            _pauseMenuUI.SetActive(false);
            _player.InputManager.SetPlayerInputMap("Gameplay");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Pause()
        {
            if (_allowMenuChange)
            {
                _allowMenuChange = false;
                _pauseMenuUI.SetActive(true);
                _player.InputManager.SetPlayerInputMap("UI");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }


        public void QuitGame()
        {
            SceneManager.LoadScene("Scenes/HomeMenu", LoadSceneMode.Single);
            _ = SessionManager.Singleton.LeaveSession();
        }
    }
}