using UnityEngine;
using System;
using Unity.Services.Multiplayer;
using Unity.Services.Authentication;
using Unity.Services.Core;
using TMPro;
using Unity.Netcode;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

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

    public ISession ActiveSession
    {
        get => activeSession;
        set => activeSession = value;
    }

    public async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch
        {
        }
    }

    public async Task StartSessionAsHost()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch
        {
        }

        SessionOptions options = new SessionOptions { MaxPlayers = 2, IsLocked = false, IsPrivate = true }.WithRelayNetwork();

        ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
        GameObject.Find("StartMenu/WelcomeText").GetComponent<TextMeshProUGUI>().text = $"Enter the Subway\nWelcome to the station {ActiveSession.Code}";
        GameObject.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text = $"Connected players: 1/2";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(GameObject.Find("StartMenu/BackButton").gameObject);
    }

    public async void JoinSession(string joinCode)
    {
        Start();

        ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(GameObject.Find("StartMenu/BackButton").gameObject);
    }

    public async void KickPlayer()
    {
        if (!ActiveSession.IsHost) return;
        string idToKick = ActiveSession.Players[0].Id == ActiveSession.Id ? ActiveSession.Players[1].Id : ActiveSession.Players[0].Id;
        await ActiveSession.AsHost().RemovePlayerAsync(idToKick);
        Debug.Log($"Player {idToKick} kicked !");
    }

    public async void LeaveSession()
    {
        if (ActiveSession != null)
        {
            try
            {
                await ActiveSession.LeaveAsync();
                GameObject.Find("DisableOnSpawn").gameObject.SetActive(true);
            }
            catch { }
            finally
            {
                activeSession = null;
                Debug.Log("Session left !");
            }
        }
    }
}