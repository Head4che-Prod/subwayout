using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Prefabs.GameManagers;
using Prefabs.Player;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace HomeMenu
{
    public class HomeMenu : MonoBehaviour
    {
        SessionManager _sessionManager;
        private GameObject _disableOnSpawn;
        private bool _isCursorActive = true;
        public static float? Time { get; set; } = null;

        private GameObject _traveling;
        private GameObject _error;
        private GameObject _waiting;
        private GameObject _tick = null;

        void Start()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
                NetworkManager.Singleton.Shutdown();


            if (SessionManager.Singleton.ActiveSession != null)
            {
                SessionManager.Singleton.Reset();
            }

            // Sign out of Unity Authentication
            try
            {
                AuthenticationService.Instance.SignOut();
            }
            catch
            {
            }

            try
            {
                GameObject.Find("DisableOnSpawn").gameObject.SetActive(true);
            }
            catch
            {
            }


            SetLang();
            _sessionManager = SessionManager.Singleton;
            _disableOnSpawn = GameObject.Find("DisableOnSpawn");
            _isCursorActive = true;
            SceneManager.activeSceneChanged += (from, to) =>
            {
                _isCursorActive = to.name == "HomeMenu" || to.name == "PlayerSelection";
                Cursor.lockState = _isCursorActive ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = _isCursorActive;
            };
            
            if (Time != null)
                OpenWinningMenu();
        }

        void Update()
        {
            Cursor.lockState = _isCursorActive ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _isCursorActive;
        }

        public void OpenCreditsMenu()
        {
            
            transform.Find("MainMenu").gameObject.SetActive(false);
            transform.Find("CreditsMenu").gameObject.SetActive(true);
            foreach (Selectable selectable in Selectable.allSelectablesArray)
                if (selectable.name == "BackButton")
                    selectable.Select();
        }

        public void CloseCreditsMenu()
        {
            transform.Find("CreditsMenu").gameObject.SetActive(false);
            transform.Find("MainMenu").gameObject.SetActive(true);
            foreach (Selectable selectable in Selectable.allSelectablesArray)
                if (selectable.name == "CreditsButton")
                    selectable.Select();
        }

        public void Quit()
        {
            #if UNITY_EDITOR
                return;
            #endif
            Application.Quit();
            Process.GetCurrentProcess().Kill();
        }

        public void OpenWinningMenu()
        {
            transform.Find("MainMenu").gameObject.SetActive(false);
            transform.Find("WinningMenu").gameObject.SetActive(true);
            foreach (Selectable selectable in Selectable.allSelectablesArray)
                if (selectable.name == "BackButton")
                    selectable.Select();

            transform.Find("WinningMenu/TimeText").GetComponent<TextMeshProUGUI>().text = TimeToString((int)(Time ?? 0));
            
            Time = null;
        }

        private static string TimeToString(int time)
        {
            string res = $"{time % 60:00}s";
            if (time < 60)
                return res;
            time /= 60;
            res = $"{time % 60:00}min " + res;
            if (time < 60)
                return res;
            time /= 60;
            return $"{time}h " + res;
        }
        
        public void CloseWinningMenu()
        {
            transform.Find("WinningMenu").gameObject.SetActive(false);
            transform.Find("MainMenu").gameObject.SetActive(true);
            foreach (Selectable selectable in Selectable.allSelectablesArray)
                if (selectable.name == "StartButton")
                    selectable.Select();
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
            _sessionManager.AddOnClientConnectedCallback((id) =>
            {
                try
                {
                    SetInteractibleStartButtons(0);
                    _disableOnSpawn.SetActive(false);
                }
                catch { }
            });
            _sessionManager.StartSessionAsHost().ContinueWith((task) =>
            {
                SetInteractibleStartButtons(0);
                _sessionManager.AddOnPlayerJoined((_) => SetInteractibleStartButtons(0));
                _sessionManager.AddOnPlayerLeft((_) => SetInteractibleStartButtons(-1));
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void CloseStart()
        {
            // Close server / Lobby
            _ = _sessionManager.LeaveSession();
            _isCursorActive = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // Reset the join code

            SceneManager.LoadScene("scenes/HomeMenu");

            // disableOnSpawn.SetActive(true);
            // // transform.Find("StartMenu/WelcomeText").GetComponent<TextMeshProUGUI>().text = "Enter the Subway\nLoading your station number...";
            // transform.Find("StartMenu/ConnectedPlayersText").GetComponent<TextMeshProUGUI>().text =
            //     "Connected players: 0/2";

            // transform.Find("StartMenu").gameObject.SetActive(false);
            // transform.Find("MainMenu").gameObject.SetActive(true);
            // foreach (Selectable selectable in Selectable.allSelectablesArray)
            //     if (selectable.name == "StartButton")
            //         selectable.Select();
        }

        public void SetInteractibleStartButtons(int dnp)
        {
            if (_sessionManager.ActiveSession == null)
            {
                transform.Find("StartMenu/MultiButton").GetComponent<Button>().interactable = false;
                transform.Find("StartMenu/SoloButton").GetComponent<Button>().interactable = false;
            }
            else
            {
                transform.Find("StartMenu/MultiButton").GetComponent<Button>().interactable =
                    (_sessionManager.ActiveSession.PlayerCount + dnp) >= 2;
                transform.Find("StartMenu/SoloButton").GetComponent<Button>().interactable =
                    (_sessionManager.ActiveSession.PlayerCount + dnp) >= 1;
            }
        }

        public void Play()
        {
            SetInteractibleStartButtons(-10);
            _sessionManager.ActiveSession.AsHost().IsLocked = true;
            _isCursorActive = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            NetworkManager.Singleton.SceneManager.LoadScene("Scenes/DemoScene", LoadSceneMode.Single);
        }

        public void PlayAlone()
        {
            SetInteractibleStartButtons(-10);

            _sessionManager.KickPlayer().ContinueWith((_) => Play(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Join()
        {
            _sessionManager.AddOnClientConnectedCallback((id) =>
            {
                try
                {
                    _disableOnSpawn.SetActive(false);
                }
                catch
                {
                    // The game already started
                }
            });
            _sessionManager.AddOnClientDisconnectedCallback((id) =>
            {
                if (id == NetworkManager.Singleton.LocalClientId)
                {
                    SceneManager.LoadScene("Scenes/HomeMenu", LoadSceneMode.Single);
                    try
                    {
                        CloseStart();
                        CloseWaitingForHostScreen();
                        CloseJoin();
                    } catch { }
                }
            });

            string joinCode = transform.Find("JoinMenu/JoinCodeInput").GetComponent<TMP_InputField>().text.ToUpper();

            transform.Find("JoinMenu").gameObject.SetActive(false);
            transform.Find("WaitingForHostScreen").gameObject.SetActive(true);
            foreach (Selectable selectable in Selectable.allSelectablesArray)
                if (selectable.name == "BackButton")
                    selectable.Select();

            _traveling = GameObject.Find("WaitingForHostScreen/Texts/Traveling");
            _traveling.SetActive(true);
            _error = GameObject.Find("WaitingForHostScreen/Texts/Error");
            _error.SetActive(false);
            _waiting = GameObject.Find("WaitingForHostScreen/Texts/Waiting");
            _waiting.SetActive(false);

            _sessionManager.JoinSession(joinCode).ContinueWith((task) =>
            {
                if (task.IsFaulted)
                {
                    _traveling.SetActive(false);
                    _waiting.SetActive(false);
                    _error.SetActive(true);
                }
                else
                {
                    _traveling.SetActive(false);
                    _waiting.SetActive(true);
                    _error.SetActive(false);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith((t) => Debug.Log(t.IsFaulted));
        }


        public void OpenSettings()
        {
            transform.Find("MainMenu").gameObject.SetActive(false);
            transform.Find("SettingsMenu").gameObject.SetActive(true);
            foreach (Selectable button in Selectable.allSelectablesArray)
                if (button.name == "BackButton")
                    button.Select();
            
            transform.Find("SettingsMenu/ShowHintsSettings/Toggle").GetComponent<Toggle>().isOn = PlayerObject.DisplayHints;
            transform.Find("SettingsMenu/SensiSettings/Slider").GetComponent<Slider>().value = PlayerCam.Sensi;
            transform.Find("SettingsMenu/LightSettings/Slider").GetComponent<Slider>().value = RenderSettings.ambientIntensity;
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
            _sessionManager.LeaveSession().ContinueWith((_) => { _disableOnSpawn.SetActive(true); },
                TaskScheduler.FromCurrentSynchronizationContext());
            _traveling.SetActive(true);
            _waiting.SetActive(true);
            _error.SetActive(true);
            transform.Find("WaitingForHostScreen").gameObject.SetActive(false);
            transform.Find("JoinMenu").gameObject.SetActive(true);
            foreach (Selectable selectable in Selectable.allSelectablesArray)
                if (selectable.name == "JoinButton")
                    selectable.Select();
        }

        public void SetLangFr()
        {
            LocalizationSettings.SelectedLocale = Locale.CreateLocale("fr-FR");
        }

        public void SetLangEn()
        {
            LocalizationSettings.SelectedLocale = Locale.CreateLocale("en-US");
        }

        /*
        public void SetLangEs()
        {
            LocalizationSettings.SelectedLocale = Locale.CreateLocale("es-ES");
        }
        */

        private void SetLang()
        {
            if (LocalizationSettings.SelectedLocale.Identifier.Code.Contains("fr"))
                SetLangFr();
            // else if (LocalizationSettings.SelectedLocale.Identifier.Code.Contains("es"))
            //     SetLangEs();
            else
                SetLangEn();
        }

        public void OpenSkinSelector()
        {
            SceneManager.LoadScene("Scenes/PlayerSelection", LoadSceneMode.Single);
        }

        public void SetSensi(Single n)
        {
            PlayerCam.Sensi = n;
        }
        
        /// <summary>
        /// Set the value of the gamma, on non-linear scale. 
        /// </summary>
        /// <param name="val">Value of gamma</param>
        public void SetGamma(float val) => BrightnessController.Instance.Value = val - 0.25f; // 0.25f is for slider correction
        
        public void SetVol(Single n)
        {
            // Todo
        }

        public void ChangeDisplayHints(bool activate)
        {
            PlayerObject.DisplayHints = activate;
            if (_tick is null)
                _tick = transform.Find("SettingsMenu/ShowHintsSettings/Toggle/Background/Checkmark").gameObject;
            _tick.SetActive(activate);
        }
    }
}