using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Prefabs.Player.PlayerUI.PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        private PlayerObject _player;
        private InputAction _pauseAction;
        private InputAction _unpauseAction;

        private GameObject _pauseMenuUI;
        private GameObject _gameElements;

        private bool _allowMenuChange;      // Prevents multi-trigger
        private List<Selectable> _toReactivate;

        private void SetGameElementsAttribute()
        {
            foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (obj.name == "GameElements")
                {
                    _gameElements = obj;
                    return;
                }
            }
        }

        void Awake()
        {
            _pauseMenuUI = transform.Find("PauseMenuUI").gameObject;
            SetGameElementsAttribute();
            _pauseMenuUI.SetActive(false);
            _gameElements.SetActive(true);
            _player = GetComponentInParent<PlayerObject>();
            _pauseAction = _player.Input.actions["Pause"];
            _unpauseAction = _player.Input.actions["Cancel"];
            _pauseAction.performed += _ => Pause();
            _unpauseAction.performed += _ => Resume();
            _toReactivate = new List<Selectable>();
            _allowMenuChange = true;
        }

        public void Resume()
        {
            if (_allowMenuChange)
            {
                _allowMenuChange = false;

                _pauseMenuUI.SetActive(false);
                _gameElements.SetActive(true);
                _player.InputManager.SetPlayerInputMap("Gameplay");
                Time.timeScale = 1f; // Resume game time
                foreach (Selectable selectable in _toReactivate)
                {
                    selectable.interactable = true;
                }
                
                StartCoroutine(WaitForEscapeReleased());
            }
        }

        void Pause()
        {
            if (_allowMenuChange)
            {
                _allowMenuChange = false;
                
                _pauseMenuUI.SetActive(true);
                _gameElements.SetActive(false);
                _player.InputManager.SetPlayerInputMap("UI");
                Time.timeScale = 0f; // Freeze game time
                _toReactivate = new List<Selectable>();
                foreach (Selectable button in Selectable.allSelectablesArray)
                    if (button.name == "ResumeButton")
                        button.Select();
                    else if (!button.transform.IsChildOf(_pauseMenuUI.transform))
                    {
                        _toReactivate.Add(button);
                        button.interactable = false;
                    }
                
                StartCoroutine(WaitForEscapeReleased());
            }
        }

        private IEnumerator WaitForEscapeReleased()
        {
            while (Keyboard.current.escapeKey.isPressed)
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