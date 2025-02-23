using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuOpener : MonoBehaviour
{
    private GameObject _pauseMenuUI;
    private GameObject _gameElements;

    private bool _isPaused = false;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (_isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        _pauseMenuUI.SetActive(false);
        _gameElements.SetActive(true);
        Time.timeScale = 1f; // Resume game time
        _isPaused = false;
    }

    void Pause()
    {
        _pauseMenuUI.SetActive(true);
        _gameElements.SetActive(false);
        Time.timeScale = 0f; // Freeze game time
        _isPaused = true;
        foreach (Selectable button in Selectable.allSelectablesArray)
            if (button.name == "ResumeButton")
                button.Select();
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Scenes/DemoMenu");
    }
}
