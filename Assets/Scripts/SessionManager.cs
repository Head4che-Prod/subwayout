using UnityEngine;
using System;
using Unity.Services.Multiplayer;
using Unity.Services.Authentication;
using Unity.Services.Core;
using TMPro;
using Unity.Netcode;

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
        catch (Exception e)
        {
            Debug.LogError($"Error initializing Unity Services: {e}");
        }
    }

    public async void StartSessionAsHost()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing Unity Services: {e}");
        }

        SessionOptions options = new SessionOptions { MaxPlayers = 2, IsLocked = false, IsPrivate = true }.WithRelayNetwork();

        ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
        Debug.Log($"Session {ActiveSession.Id} created ! Join code : {ActiveSession.Code}");
        GameObject.Find("StartMenu/WelcomeText").GetComponent<TextMeshProUGUI>().text = $"Enter the Subway\nWelcome to the station {ActiveSession.Code}";
    }

    public async void JoinSession(string joinCode)
    {
        ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
        Debug.Log($"Session {ActiveSession.Id} joined !");
        NetworkManager.Singleton.AddNetworkPrefab( Resources.Load<GameObject>("NetworkPlayer") );
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