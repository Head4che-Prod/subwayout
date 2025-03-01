using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Prefabs.Player.PlayerUI.DebugConsole
{
    public class DebugConsole : MonoBehaviour
    {
        public static DebugConsole Singleton;
        [SerializeField] Text displayText;
        [SerializeField] TMP_InputField inputField;
        // Needs to be initialized here in order to avoid NullReferenceExceptions when adding commands in other files
        private static readonly Dictionary<string, Action> Commands = new Dictionary<string, Action>();
        private List<string> _commandHistory;
        private int _commandHistoryIndex;
        private string _tempCommand;
        private bool _ignoreNextInput = false;

        public static bool IsActivated { get; private set; }
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
                Commands["inputMode"] = () => Log(_previousInputMap);
                Commands["help"] = () => Log("Available commands:\n - " + String.Join("\n - ", Commands.Keys));
            }

            if (this != null && gameObject != null && gameObject.transform != null && gameObject.transform.GetChild(0) != null && gameObject.transform.GetChild(0).gameObject != null)
                gameObject.transform.GetChild(0).gameObject.SetActive(_isActivated);
        }

        public void Start()
        {
            _player = GetComponentInParent<PlayerObject>();
            IsActivated = false;

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

        private void ToggleConsole(InputAction.CallbackContext ctx)
        {
            IsActivated = !IsActivated;
            gameObject.transform.GetChild(0).gameObject.SetActive(IsActivated);
        }

        private void FocusOnConsole(InputAction.CallbackContext context)
        {
            if (IsActivated)
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
            inputField.DeactivateInputField();
            Keyboard.current.onTextInput -= HandleCommandInput;
        }

        private void HandleCommandInput(char c)
        {
            if ((Char.IsLetter(c) || c == ' ') && !_ignoreNextInput)
            {
                if (!(Keyboard.current.shiftKey.isPressed ^
                      Keyboard.current.capsLockKey.isPressed)) // If caps lock XNOR shift
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

        public void NavigateUpHistory()
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

        public void NavigateDownHistory()
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

        public void ExecCommand()
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
    }
}
