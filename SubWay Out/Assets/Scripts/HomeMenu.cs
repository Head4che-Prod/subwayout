using UnityEngine;

public class HomeMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateGame()
    {
        
    }

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
}
