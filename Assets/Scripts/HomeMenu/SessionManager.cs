using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace HomeMenu
{
    public class SessionManager
    {
        private static SessionManager _singleton;

        public static SessionManager Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = new SessionManager();
                }

                return _singleton;
            }
        }

        private ISession _activeSession;
        private bool _isHost = false;
        private static List<Action<ulong>> _connectedCallBacks = new List<Action<ulong>>();
        private static List<Action<ulong>> _disconnectedCallBacks = new List<Action<ulong>>();
        private static List<Action<string>> _joinCallBacks = new List<Action<string>>();
        private static List<Action<string>> _leftCallBacks = new List<Action<string>>();
        private GameObject _loadingText;
        private GameObject _stationNumber;

        public ISession ActiveSession
        {
            get => _activeSession;
            set => _activeSession = value;
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

            foreach (Action<string> f in _joinCallBacks)
                ActiveSession.PlayerJoined -= f;
            _joinCallBacks = new List<Action<string>>();

            foreach (Action<string> f in _leftCallBacks)
                ActiveSession.PlayerLeft -= f;
            _leftCallBacks = new List<Action<string>>();

            try
            {
                await ActiveSession.LeaveAsync();
                GameObject.Find("DisableOnSpawn").gameObject.SetActive(true);
            }
            catch
            {
                // ignored
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
            _joinCallBacks.Add(f);
            ActiveSession.PlayerJoined += f;
        }

        public void AddOnPlayerLeft(Action<string> f)
        {
            _leftCallBacks.Add(f);
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
                // ignored
            }

            _isHost = false;
        }

        public async Task StartSessionAsHost()
        {
            _loadingText = GameObject.Find("StartMenu/LoadingText");
            _stationNumber = GameObject.Find("StartMenu/StationNumber");
            _stationNumber.SetActive(false);
            await Start();
            _isHost = true;

            SessionOptions options =
                new SessionOptions { MaxPlayers = 2, IsLocked = false, IsPrivate = true }.WithRelayNetwork();

            ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
            _stationNumber.SetActive(true);
            _loadingText.SetActive(false);
            string oldText = _stationNumber.GetComponent<TextMeshProUGUI>().text;
            _stationNumber.GetComponent<TextMeshProUGUI>().text = oldText[..^6] + ActiveSession.Code;
            GameObject.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text = GameObject.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text[..^3] +
                "1/2";
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            EventSystem.current.SetSelectedGameObject(GameObject.Find("StartMenu/BackButton").gameObject);

            AddOnPlayerJoined((id) =>
            {
                GameObject textObject = GameObject.Find("StartMenu/ConnectedPlayersText");
                if (textObject != null)
                {
                    GameObject.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text =
                    GameObject.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text[..^3] + $"{ActiveSession.PlayerCount}/2";
                }
            });
            AddOnPlayerLeft((id) =>
            {
                GameObject textObject = GameObject.Find("StartMenu/ConnectedPlayersText");
                if (textObject != null)
                {
                    textObject.GetComponent<TextMeshProUGUI>().text =
                        GameObject.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text[..^3] + $"{ActiveSession.PlayerCount - 1}/2";
                }
            });
        }

        public async Task JoinSession(string joinCode)
        {
            await Start();
            _isHost = false;
            ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            EventSystem.current.SetSelectedGameObject(GameObject.Find("BackButton").gameObject);
            if (_activeSession == null)
                throw new ArgumentException("Wrong code or no connection");
        }

        public async Task KickPlayer()
        {
            if (!_isHost || ActiveSession.Players.Count == 1) return;
            string idToKick = ActiveSession.Players[1].Id;
            await ActiveSession.AsHost().RemovePlayerAsync(idToKick);
            Debug.Log($"Player {idToKick} kicked !");
        }

        public async Task LeaveSession()
        {
            _isHost = false;
            if (ActiveSession != null)
            {
                try
                {
                    await ActiveSession.LeaveAsync();
                    GameObject.Find("DisableOnSpawn").gameObject.SetActive(true);
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    _activeSession = null;
                    Debug.Log("Session left !");
                }
            }
        }
    }
}