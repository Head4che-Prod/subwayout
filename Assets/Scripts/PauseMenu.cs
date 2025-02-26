using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuOpener : MonoBehaviour
{
    private GameObject pauseMenuUI;
    [SerializeField]
    public GameObject gameElements;

    private bool isPaused = false;

    void Awake() 
    {
        pauseMenuUI = transform.Find("PauseMenuUI").gameObject;
        pauseMenuUI.SetActive(false);
        gameElements.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        gameElements.SetActive(true);
        Time.timeScale = 1f; // Resume game time
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        gameElements.SetActive(false);
        Time.timeScale = 0f; // Freeze game time
        isPaused = true;
        EventSystem.current.SetSelectedGameObject(null);
        foreach (Selectable button in Selectable.allSelectablesArray)
            if (button.name == "ResumeButton")
                button.Select();
    }

    public void QuitGame()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Scenes/DemoMenu");
    }
}
