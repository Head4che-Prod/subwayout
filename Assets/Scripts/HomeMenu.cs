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
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeMenu : MonoBehaviour
{
    SessionManager sessionManager;
    private GameObject disableOnSpawn;
    [SerializeField] public GameObject PlayerPrefab;
    private bool _isCursorActive = true;

    void Start()
    {
        SetLang();
        sessionManager = SessionManager.Singleton;
        disableOnSpawn = GameObject.Find("DisableOnSpawn");
        _isCursorActive = true;
        SceneManager.activeSceneChanged += (from, to) => {
            _isCursorActive = false;
            Cursor.lockState = _isCursorActive? CursorLockMode.None: CursorLockMode.Locked;
            Cursor.visible = _isCursorActive;
        };
    }

    void Update()
    {
        Cursor.lockState = _isCursorActive? CursorLockMode.None: CursorLockMode.Locked;
        Cursor.visible = _isCursorActive;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenStart()
    {
        transform.Find("MainMenu").gameObject.SetActive(false);
        transform.Find("StartMenu").gameObject.SetActive(true);
        SetInteractibleStartButtons(0);
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
            SetInteractibleStartButtons(0);
            disableOnSpawn.SetActive(false);
        };
        sessionManager.StartSessionAsHost().ContinueWith((task) =>
        {
            SetInteractibleStartButtons(0);
            sessionManager.ActiveSession.PlayerJoined += (_) => SetInteractibleStartButtons(0);
            sessionManager.ActiveSession.PlayerLeft += (_) => SetInteractibleStartButtons(-1);
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void CloseStart()
    {
        // Close server / Lobby
        _ = sessionManager.LeaveSession();
        _isCursorActive = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Reset the join code

        disableOnSpawn.SetActive(true);
        // transform.Find("StartMenu/WelcomeText").GetComponent<TextMeshProUGUI>().text = "Enter the Subway\nLoading your station number...";
        transform.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text = "Connected players: 0/2";

        transform.Find("StartMenu").gameObject.SetActive(false);
        transform.Find("MainMenu").gameObject.SetActive(true);
        foreach (Selectable selectable in Selectable.allSelectablesArray)
            if (selectable.name == "StartButton")
                selectable.Select();
    }

    public void SetInteractibleStartButtons(int dnp)
    {
        if (sessionManager.ActiveSession == null)
        {
            transform.Find("StartMenu/MultiButton").GetComponent<Button>().interactable = false;
            transform.Find("StartMenu/SoloButton").GetComponent<Button>().interactable = false;
        }
        else
        {
            transform.Find("StartMenu/MultiButton").GetComponent<Button>().interactable = (sessionManager.ActiveSession.PlayerCount + dnp) >= 2;
            transform.Find("StartMenu/SoloButton").GetComponent<Button>().interactable = (sessionManager.ActiveSession.PlayerCount + dnp) >= 1;
        }
    }

    public void Play()
    {
        SetInteractibleStartButtons(-10);
        sessionManager.ActiveSession.AsHost().IsLocked = true;
        _isCursorActive = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        NetworkManager.Singleton.SceneManager.LoadScene("Scenes/DemoScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void PlayAlone()
    {
        SetInteractibleStartButtons(-10);

        sessionManager.KickPlayer().ContinueWith((_) => Play(), TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void Join()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            disableOnSpawn.SetActive(false);
        };
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (id == NetworkManager.Singleton.LocalClientId)
            {
                Debug.Log("Local player disconnected!");
                SceneManager.LoadScene("Scenes/HomeMenu", LoadSceneMode.Single);
                CloseStart();
                CloseWaitingForHostScreen();
                CloseJoin();
            }
        };

        string joinCode = transform.Find("JoinMenu/JoinCodeInput").GetComponent<TMP_InputField>().text.ToUpper();

        sessionManager.JoinSession(joinCode).ContinueWith((task) =>
        {
            if (task.IsFaulted)
                SetConnectedStatus("<color=red>Impossible to find the station. Check your connection and the code you entered.");
            else
                SetConnectedStatus("Connected ! Waiting for your host to start the game...");
        }, TaskScheduler.FromCurrentSynchronizationContext());

        transform.Find("JoinMenu").gameObject.SetActive(false);
        transform.Find("WaitingForHostScreen").gameObject.SetActive(true);
        foreach (Selectable selectable in Selectable.allSelectablesArray)
            if (selectable.name == "BackButton")
                selectable.Select();

        SetConnectedStatus("Traveling to this station...");
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
        sessionManager.LeaveSession().ContinueWith((_) => { disableOnSpawn.SetActive(true); }, TaskScheduler.FromCurrentSynchronizationContext());
        SetConnectedStatus("Traveling to this station...");
        transform.Find("WaitingForHostScreen").gameObject.SetActive(false);
        transform.Find("JoinMenu").gameObject.SetActive(true);
        foreach (Selectable selectable in Selectable.allSelectablesArray)
            if (selectable.name == "JoinButton")
                selectable.Select();
    }

    private void SetConnectedStatus(string txt)
    {
        transform.Find("WaitingForHostScreen/TextPlay").GetComponent<TMP_Text>().text = txt;
    }

    public void SetLangFr() {
        LocalizationSettings.SelectedLocale = Locale.CreateLocale("fr-FR");
    }

    public void SetLangEn() {
        LocalizationSettings.SelectedLocale = Locale.CreateLocale("en-US");
    }

    public void SetLangEs() {
        LocalizationSettings.SelectedLocale = Locale.CreateLocale("es-ES");
    }

    private void SetLang() {
        if (LocalizationSettings.SelectedLocale.Identifier.Code.Contains("fr"))
            SetLangFr();
        else if (LocalizationSettings.SelectedLocale.Identifier.Code.Contains("es"))
            SetLangEs();
        else
            SetLangEn();
        }
}