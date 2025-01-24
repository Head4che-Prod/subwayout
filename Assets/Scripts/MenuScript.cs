using UnityEngine;

public class MenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Quit()
    {
        Application.Quit();
    }
    
    public void DemoHanoi()
    {
        // UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void DemoModels()
    {
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MultiplayerConnection");
    }

    public void Settings()
    {
        // UnityEngine.SceneManagement.SceneManager.LoadScene("OptionsPage");
    }
}
