using UnityEngine;
using UnityEngine.UI;

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
		transform.Find("SettingsMenu").gameObject.SetActive(true);
        foreach (Selectable button in Selectable.allSelectablesArray)
            if (button.name == "BackButton")
                button.Select();
    }

    public void CloseSettings()
    {
		transform.Find("SettingsMenu").gameObject.SetActive(false);
		transform.Find("MainMenu").gameObject.SetActive(true);
        foreach (Selectable button in Selectable.allSelectablesArray)
            if (button.name == "OptionsButton")
                button.Select();
    }
}
