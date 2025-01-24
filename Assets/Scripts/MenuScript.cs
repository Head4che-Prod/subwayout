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
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/DemoHanoi");
    }

    public void DemoModels()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/GameScene");
    }

    public void OpenSettings()
    {
		transform.Find("MainMenu").gameObject.SetActive(false);
		transform.Find("AppearOnSelect").gameObject.SetActive(true);
    }
}
