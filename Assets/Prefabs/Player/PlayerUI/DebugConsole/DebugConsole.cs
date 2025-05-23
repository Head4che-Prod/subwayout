using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Prefabs.Player.PlayerUI.DebugConsole
{
    public class DebugConsole : MonoBehaviour
    {
        public static DebugConsole Singleton;
        [SerializeField] TMP_Text displayText;
        [SerializeField] TMP_InputField inputField;
        // Needs to be initialized here in order to avoid NullReferenceExceptions when adding commands in other files
        private static readonly Dictionary<string, Action> Commands = new Dictionary<string, Action>();
        private List<string> _commandHistory;
        private int _commandHistoryIndex;
        private string _tempCommand;
        private bool _ignoreNextInput = false;

        private static bool _isActivated;
        private string _previousInputMap;
        private string _currentText;

        private PlayerObject _player;
        private InputAction _showConsoleAction;
        private InputAction _focusConsoleAction;
        private InputAction _cancelAction;
        private InputAction _submitAction;
        private InputAction _backspaceAction;
        private InputAction _upAction;
        private InputAction _downAction;

        void Awake()
        {
            if (Singleton != null)
                DestroyImmediate(gameObject);
            else
            {
                Singleton = this;

                Commands["sayHello"] = () => Log("Hello, world!");
                Commands["inputMode"] = () => Log($"Input map: {_player.Input.currentActionMap.name}; previous: {_previousInputMap}");
                Commands["loadDemoScene"] = () => NetworkManager.Singleton.SceneManager.LoadScene("Scenes/DemoScene", LoadSceneMode.Single);
                Commands["help"] = () => Log("Available commands:\n - " + String.Join("\n - ", Commands.Keys));
                Commands["exit"] = () => NetworkManager.Singleton.SceneManager.LoadScene("Scenes/HomeMenu", LoadSceneMode.Single);
            }

            if (this != null && gameObject != null && gameObject.transform != null && gameObject.transform.GetChild(0) != null && gameObject.transform.GetChild(0).gameObject != null)
                gameObject.transform.GetChild(0).gameObject.SetActive(_isActivated);
        }

        public void Start()
        {
            _player = GetComponentInParent<PlayerObject>();
            _isActivated = false;

            _showConsoleAction = _player.Input.actions["ShowConsole"];
            _focusConsoleAction = _player.Input.actions["ConsoleFocus"];
            _cancelAction = _player.Input.actions["DebugConsoleCancel"];    // Names are distinct as to not interfere with other maps
            _submitAction = _player.Input.actions["DebugConsoleSubmit"];
            _backspaceAction = _player.Input.actions["DebugConsoleBackspace"];
            _upAction = _player.Input.actions["DebugConsoleUpArrow"];
            _downAction = _player.Input.actions["DebugConsoleDownArrow"];
            _showConsoleAction.performed += ToggleConsole;
            _focusConsoleAction.performed += FocusOnConsole;
            _submitAction.performed += _ => ExecCommand();
            _cancelAction.performed += _ => FocusOffConsole();
            _backspaceAction.performed += _ => Backspace();
            _upAction.performed += _ => NavigateUpHistory();
            _downAction.performed += _ => NavigateDownHistory();
            
            
            _previousInputMap = _player.Input.currentActionMap.name;
            _commandHistory = new List<string>();
            _commandHistoryIndex = -1;
            _tempCommand = "";

            _currentText = "";
        }

        public void ToggleConsole(InputAction.CallbackContext ctx)
        {
            _isActivated = !_isActivated;
            if (!_isActivated)
                FocusOffConsole();
            gameObject.transform.GetChild(0).gameObject.SetActive(_isActivated);
        }

        private void FocusOnConsole(InputAction.CallbackContext context)
        {
            if (_isActivated)
            {
                _previousInputMap = _player.Input.currentActionMap.name;
                _player.InputManager.SetPlayerInputMap("DebugConsole");
                Keyboard.current.onTextInput += HandleCommandInput;
                _commandHistoryIndex = -1;
                _tempCommand = "";
                inputField.Select();
                _ignoreNextInput = true;
            }
        }

        private void FocusOffConsole()
        {
            _player.InputManager.SetPlayerInputMap(_previousInputMap);
            inputField.text = "";
            _currentText = "";
            inputField.enabled = false;
            Keyboard.current.onTextInput -= HandleCommandInput;
        }

        private void HandleCommandInput(char c)
        {
            if ((Char.IsLetter(c) || c == ' ') && !_ignoreNextInput)
            {
                if (!Keyboard.current.shiftKey.isPressed)
                    c = Char.ToLower(c);
                _currentText += c;
                inputField.text = _currentText;
            }

            _ignoreNextInput = false;
        }

        private void Backspace()
        {
            if (_currentText.Length > 0)
            {
                _currentText = _currentText[..^1];
                inputField.text = _currentText;
            }
        }

        private void NavigateUpHistory()
        {
            if (_commandHistoryIndex < _commandHistory.Count - 1)
            {
                if (_commandHistoryIndex == -1)
                    _tempCommand = _currentText;
                _commandHistoryIndex++;
                _currentText = _commandHistory[_commandHistoryIndex];
                inputField.text = _currentText;
            }
        }

        private void NavigateDownHistory()
        {
            if (_commandHistoryIndex > 0)
            {
                _commandHistoryIndex--;
                _currentText = _commandHistory[_commandHistoryIndex];
                inputField.text = _currentText;
            }
            else
            {
                if (_tempCommand != "")
                {
                    _currentText = _tempCommand;
                    inputField.text = _currentText;
                    _tempCommand = "";
                }
                _commandHistoryIndex = -1;
            }
        }

        private void ExecCommand()
        {
            if (_currentText == "")
                return;
            if (_currentText.Trim() != "" && (_commandHistory.Count == 0 || _currentText != _commandHistory[0]))
                _commandHistory.Insert(0, _currentText);
            Commands.GetValueOrDefault(_currentText, () => LogError("Command doesn't exist"))();
            FocusOffConsole();
        }

        public static void AddCommand(string name, Action func)
        {
            Commands[name] = func;
        }

        public void Log(string msg)
        {
            displayText.text = msg + '\n' + displayText.text;
            Debug.Log(msg);
        }

        public void LogWarning(string msg)
        {
            Log($"<color=orange>{msg}</color>");
        }

        public void LogError(string msg)
        {
            Log($"<color=red>{msg}</color>");
        }

        private void ExecCommand(InputAction.CallbackContext _) {
            ExecCommand();
        }

        private void FocusOffConsole(InputAction.CallbackContext _) {
            FocusOffConsole();
        }

        private void Backspace(InputAction.CallbackContext _) {
            Backspace();
        }

        private void NavigateUpHistory(InputAction.CallbackContext _) {
            NavigateUpHistory();
        }

        private void NavigateDownHistory(InputAction.CallbackContext _) {
            NavigateDownHistory();
        }

        public void OnDestroy()
        {
            if (_showConsoleAction != null)
                _showConsoleAction.performed -= ToggleConsole;
            if (_focusConsoleAction != null)
                _focusConsoleAction.performed -= FocusOnConsole;
            if (_submitAction != null)
                _submitAction.performed -= ExecCommand;
            if (_cancelAction != null)
                _cancelAction.performed -= FocusOffConsole;
            if (_backspaceAction != null)
                _backspaceAction.performed -= Backspace;
            if (_upAction != null)
                _upAction.performed -= NavigateUpHistory;
            if (_downAction != null)
                _downAction.performed -= NavigateDownHistory;
            if (Singleton == this)
                Singleton = null;
        }
    }
}
