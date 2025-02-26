using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Prefabs.Player.PlayerUI.PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        private PlayerObject _player;
        private InputAction _pauseAction;
        private InputAction _unpauseAction;

        private GameObject _pauseMenuUI;

        private bool _allowMenuChange;      // Prevents multi-trigger
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
            Debug.Log("Unpause");
            Debug.Log(_player.Input.currentActionMap.name);
            if (_allowMenuChange)
            {
                _allowMenuChange = false;

                foreach (DynamicButton button in buttons)
                    if (button.isActiveAndEnabled)
                        button.Deactivate();
                
                _pauseMenuUI.SetActive(false);
                _player.InputManager.SetPlayerInputMap("Gameplay");
                StartCoroutine(WaitForEscapeReleased());
            }
        }

        public void Pause()
        {
            Debug.Log("Pause");
            if (_allowMenuChange)
            {
                _allowMenuChange = false;
                _pauseMenuUI.SetActive(true);
                _player.InputManager.SetPlayerInputMap("UI");
                buttons[0].Activate();
                StartCoroutine(WaitForEscapeReleased());
            }
        }

        private IEnumerator WaitForEscapeReleased()
        {
            {
                yield return null;
            }
            _allowMenuChange = true;   // Player can now switch menu once again
        }
        
        
        public void QuitGame()
        {
            SceneManager.LoadScene("Scenes/DemoMenu");
        }
    }
}