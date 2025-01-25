using UnityEngine;

public class HomeMenu : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }
    
    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void MultiScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MultiplayerConnection");
    }

    public void OptionsScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("OptionsPage");
    }
}
