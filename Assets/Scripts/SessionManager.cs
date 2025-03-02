using UnityEngine;
using System;
using Unity.Services.Multiplayer;
using Unity.Services.Authentication;
using Unity.Services.Core;
using TMPro;
using Unity.Netcode;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SessionManager
{
    private static SessionManager singleton;

    public static SessionManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = new SessionManager();
            }

            return singleton;
        }
    }

    private ISession activeSession;
    private bool IsHost = false;
    private static List<Action<ulong>> _connectedCallBacks = new List<Action<ulong>>();
    private static List<Action<ulong>> _disconnectedCallBacks = new List<Action<ulong>>();
    private static List<Action<string>> _JoinCallBacks = new List<Action<string>>();
    private static List<Action<string>> _LeftCallBacks = new List<Action<string>>();
    private GameObject LoadingText;
    private GameObject StationNumber;

    public ISession ActiveSession
    {
        get => activeSession;
        set => activeSession = value;
    }
    public async void Reset()
    {
        foreach (InputDevice d in InputSystem.devices)
            InputSystem.ResetDevice(d);

        foreach (Action<ulong> f in _connectedCallBacks)
            NetworkManager.Singleton.OnClientConnectedCallback -= f;
        _connectedCallBacks = new List<Action<ulong>>();

        foreach (Action<ulong> f in _disconnectedCallBacks)
            NetworkManager.Singleton.OnClientDisconnectCallback -= f;
        _disconnectedCallBacks = new List<Action<ulong>>();

        foreach (Action<string> f in _JoinCallBacks)
            ActiveSession.PlayerJoined -= f;
        _JoinCallBacks = new List<Action<string>>();

        foreach (Action<string> f in _LeftCallBacks)
            ActiveSession.PlayerLeft -= f;
        _LeftCallBacks = new List<Action<string>>();

        try
        {
            await ActiveSession.LeaveAsync();
            GameObject.Find("DisableOnSpawn").gameObject.SetActive(true);
        }
        catch
        {
        }
        finally
        {
            ActiveSession = null;
            Debug.Log("Session left and NetworkManager shut down!");
        }
    }

    public void AddOnClientConnectedCallback(Action<ulong> f)
    {
        _connectedCallBacks.Add(f);
        NetworkManager.Singleton.OnClientConnectedCallback += f;
    }

    public void AddOnPlayerJoined(Action<string> f)
    {
        _JoinCallBacks.Add(f);
        ActiveSession.PlayerJoined += f;
    }

    public void AddOnPlayerLeft(Action<string> f)
    {
        _LeftCallBacks.Add(f);
        ActiveSession.PlayerLeft += f;
    }

    public void AddOnClientDisconnectedCallback(Action<ulong> f)
    {
        _disconnectedCallBacks.Add(f);
        NetworkManager.Singleton.OnClientDisconnectCallback += f;
    }

    public async Task Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch
        {
        }

        IsHost = false;
    }

    public async Task StartSessionAsHost()
    {
        LoadingText = GameObject.Find("StartMenu/LoadingText");
        StationNumber = GameObject.Find("StartMenu/StationNumber");
        StationNumber.SetActive(false);
        await Start();
        IsHost = true;

        SessionOptions options =
            new SessionOptions { MaxPlayers = 2, IsLocked = false, IsPrivate = true }.WithRelayNetwork();

        ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
        StationNumber.SetActive(true);
        LoadingText.SetActive(false);
        string oldText = StationNumber.GetComponent<TextMeshProUGUI>().text;
        StationNumber.GetComponent<TextMeshProUGUI>().text = oldText[..^6] + ActiveSession.Code;
        GameObject.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text = GameObject.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text[..^3] +
            "1/2";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(GameObject.Find("StartMenu/BackButton").gameObject);

        AddOnPlayerJoined((id) =>
        {
            GameObject.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text =
                GameObject.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text[..^3] + $"{ActiveSession.PlayerCount}/2";
        });
        AddOnPlayerLeft((id) =>
        {
            GameObject.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text =
                GameObject.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text[..^3] + $"{ActiveSession.PlayerCount - 1}/2";
        });
    }

    public async Task JoinSession(string joinCode)
    {
        await Start();
        IsHost = false;
        ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(GameObject.Find("BackButton").gameObject);
        if (activeSession == null)
            throw new ArgumentException("Wrong code or no connection");
    }

    public async Task KickPlayer()
    {
        if (!IsHost) return;
        string idToKick = ActiveSession.Players[1].Id;
        await ActiveSession.AsHost().RemovePlayerAsync(idToKick);
        Debug.Log($"Player {idToKick} kicked !");
    }

    public async Task LeaveSession()
    {
        IsHost = false;
        if (ActiveSession != null)
        {
            try
            {
                await ActiveSession.LeaveAsync();
                GameObject.Find("DisableOnSpawn").gameObject.SetActive(true);
            }
            catch
            {
            }
            finally
            {
                activeSession = null;
                Debug.Log("Session left !");
            }
        }
    }
}