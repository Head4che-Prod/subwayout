using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class HomeMenu : MonoBehaviour
{
    SessionManager sessionManager;
    private GameObject disableOnSpawn;
    [SerializeField] public GameObject PlayerPrefab;

    void Start()
    {
        sessionManager = SessionManager.Singleton;
        disableOnSpawn = GameObject.Find("DisableOnSpawn");
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenStart()
    {
        transform.Find("MainMenu").gameObject.SetActive(false);
        transform.Find("StartMenu").gameObject.SetActive(true);
        SetInteractibleStartButtons(false);
        foreach (Selectable selectable in Selectable.allSelectablesArray)
            if (selectable.name == "BackButton")
                selectable.Select();

        // Start server / Lobby

        // NetworkManager.Singleton.OnConnectionEvent += (a, b) => {
        //     Cursor.lockState = CursorLockMode.Confined;
        //     Cursor.visible = true;
        // };
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            SetInteractibleStartButtons(true);
            disableOnSpawn.SetActive(false);
        };
        _ = sessionManager.StartSessionAsHost();
    }

    public void CloseStart()
    {
        // Close server / Lobby
        sessionManager.LeaveSession();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Reset the join code

        disableOnSpawn.SetActive(true);
        transform.Find("StartMenu/WelcomeText").GetComponent<TextMeshProUGUI>().text = "Enter the Subway\nLoading your station number...";
        transform.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text = "Connected players: 0/2";

        transform.Find("StartMenu").gameObject.SetActive(false);
        transform.Find("MainMenu").gameObject.SetActive(true);
        foreach (Selectable selectable in Selectable.allSelectablesArray)
            if (selectable.name == "StartButton")
                selectable.Select();
    }

    public void SetInteractibleStartButtons(bool interactible)
    {
        transform.Find("StartMenu/MultiButton").GetComponent<Button>().interactable = interactible;
        transform.Find("StartMenu/SoloButton").GetComponent<Button>().interactable = interactible;
    }

    public void Play()
    {
        SetInteractibleStartButtons(false);
        sessionManager.ActiveSession.AsHost().IsLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        NetworkManager.Singleton.SceneManager.LoadScene("Scenes/TempHanoi", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void PlayAlone()
    {
        SetInteractibleStartButtons(false);

        sessionManager.KickPlayer();
        Play();
    }

    public void Join()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            disableOnSpawn.SetActive(false);
        };
        string joinCode = transform.Find("JoinMenu/JoinCodeInput").GetComponent<TMP_InputField>().text.ToUpper();
        sessionManager.JoinSession(joinCode);
        transform.Find("JoinMenu").gameObject.SetActive(false);
        transform.Find("WaitingForHostScreen").gameObject.SetActive(true);
        foreach (Selectable selectable in Selectable.allSelectablesArray)
            if (selectable.name == "BackButton")
                selectable.Select();
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
        foreach (Selectable selectable in Selectable.allSelectablesArray)
            if (selectable.name == "OptionsButton")
                selectable.Select();
    }

    public void OpenJoin()
    {
        transform.Find("MainMenu").gameObject.SetActive(false);
        transform.Find("JoinMenu").gameObject.SetActive(true);
        foreach (Selectable selectable in Selectable.allSelectablesArray)
            if (selectable.name == "JoinCodeInput")
                selectable.Select();
    }

    public void CloseJoin()
    {
        transform.Find("JoinMenu").gameObject.SetActive(false);
        transform.Find("MainMenu").gameObject.SetActive(true);
        foreach (Selectable selectable in Selectable.allSelectablesArray)
            if (selectable.name == "JoinButton")
                selectable.Select();
    }

    public void CloseWaitingForHostScreen()
    {
        sessionManager.LeaveSession();
        transform.Find("WaitingForHostScreen").gameObject.SetActive(false);
        transform.Find("JoinMenu").gameObject.SetActive(true);
        foreach (Selectable selectable in Selectable.allSelectablesArray)
            if (selectable.name == "JoinButton")
                selectable.Select();
    }
}
