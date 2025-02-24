using System.Collections.Generic;
using Prefabs.Player;
using Prefabs.Player.PlayerUI.DebugConsole;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Prefabs.UI.PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        private InputAction _pauseAction;

        private GameObject _pauseMenuUI;
        private GameObject _gameElements;

        private bool _isPaused = false;
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
            _pauseAction = GetComponentInParent<PlayerObject>().Input.actions["Pause"];
            _pauseAction.performed += _ =>
            {
                if (_isPaused)
                    Resume();
                else
                    Pause();
            };
            _toReactivate = new List<Selectable>();
        }

        public void Resume()
        {
            _pauseMenuUI.SetActive(false);
            _gameElements.SetActive(true);
            Time.timeScale = 1f; // Resume game time
            _isPaused = false;
            foreach (Selectable selectable in _toReactivate)
            {
                selectable.interactable = true;
            }
        }

        void Pause()
        {
            _pauseMenuUI.SetActive(true);
            _gameElements.SetActive(false);
            Time.timeScale = 0f; // Freeze game time
            _isPaused = true;
            _toReactivate = new List<Selectable>();
            foreach (Selectable button in Selectable.allSelectablesArray)
                if (button.name == "ResumeButton")
                    button.Select();
                else if (!button.transform.IsChildOf(_pauseMenuUI.transform))
                {
                    _toReactivate.Add(button);
                    button.interactable = false;
                }
        }

        public void QuitGame()
        {
            SceneManager.LoadScene("Scenes/DemoMenu");
        }
    }
}