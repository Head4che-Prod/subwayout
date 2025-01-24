using UnityEngine;

public class PauseMenuOpener : MonoBehaviour
{
    public GameObject pauseMenuUI; // Reference to your Pause Menu UI

    private bool isPaused = false;

    void Awake() 
    {
        pauseMenuUI = transform.Find("PauseMenu").gameObject;
        pauseMenuUI.SetActive(false);
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
        // Disable the pause menu UI and un-pause the game
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume game time
        isPaused = false;
    }

    void Pause()
    {
        // Enable the pause menu UI and pause the game
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freeze game time
        isPaused = true;
    }

    public void QuitGame()
    {
        // Quit the application (works in a built game, not in the editor)
        Debug.Log("Quitting the game...");
        Application.Quit();
    }
}
