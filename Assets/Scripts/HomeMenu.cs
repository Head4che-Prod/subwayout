using UnityEngine;

public class HomeMenu : MonoBehaviour
{
    private float delayBetweenPresses = 0.25f;
    private int pressCounter = 0;
    private float lastPressedTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pressCounter = 0;
    }

    void Update()
    {
        if (Time.time - lastPressedTime >= delayBetweenPresses)
        {
            pressCounter = 0;
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if (pressCounter != 0)
                pressCounter++;
            else
                pressCounter = 1;

            lastPressedTime = Time.time;
        }

        if (pressCounter >= 4)
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/NetworkDebug");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/GameScene");
    }

    public void MultiScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/MultiplayerConnection");
    }

    public void OptionsScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/OptionsPage");
    }
}