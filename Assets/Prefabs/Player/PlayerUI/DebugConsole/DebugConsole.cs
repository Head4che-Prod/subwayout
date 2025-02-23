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
        private static bool _isActivated;
        private string _previousInputMap;
        private string _currentText;

        private PlayerObject _player;
        private InputAction _showConsoleAction;
        private InputAction _focusConsoleAction;
        private InputAction _cancelAction;
        private InputAction _submitAction;
        private InputAction _backspaceAction;

        void Awake()
        {
            if (Singleton != null)
                DestroyImmediate(gameObject);
            else
            {
                Singleton = this;

                Commands.Add("sayHello", () => Log("Hello, world!"));
                Commands.Add("inputMode", () => Log(_player.Input.currentActionMap.name));
                Commands.Add("help", () => Log("Available commands:\n - " + String.Join("\n - ", Commands.Keys)));
            }

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
            _showConsoleAction.performed += ToggleConsole;
            _focusConsoleAction.performed += FocusOnConsole;
            _submitAction.performed += _ => ExecCommand();
            _cancelAction.performed += _ => FocusOffConsole();
            _backspaceAction.performed += _ => Backspace();
            
            _previousInputMap = _player.Input.currentActionMap.name;

            _currentText = "";
        }

        private void ToggleConsole(InputAction.CallbackContext ctx)
        {
            _isActivated = !_isActivated;   
            gameObject.transform.GetChild(0).gameObject.SetActive(_isActivated);
        }

        private void FocusOnConsole(InputAction.CallbackContext context)
        {
            if (_isActivated)
            {
                inputField.Select();
                _previousInputMap = _player.Input.currentActionMap.name;
                _player.InputManager.SetPlayerInputMap("DebugConsole");
                Keyboard.current.onTextInput += HandleCommandInput;
            }
        }

        private void FocusOffConsole() 
        {
            Log("Switching back to " + _previousInputMap);
            _player.InputManager.SetPlayerInputMap(_previousInputMap);
            inputField.text = "";
            _currentText = "";
            inputField.DeactivateInputField();
            Keyboard.current.onTextInput -= HandleCommandInput;
        }

        private void HandleCommandInput(char c)
        {
            if (Char.IsLetter(c) || c == ' ')
            {
                if (!(Keyboard.current.shiftKey.isPressed ^
                      Keyboard.current.capsLockKey.isPressed)) // If caps lock XNOR shift
                    c = Char.ToLower(c);
                _currentText += c;
                inputField.text = _currentText;
            }
        }

        private void Backspace()
        {
            if (_currentText.Length > 0)
            {
                _currentText = _currentText[..^1];
                inputField.text = _currentText;
            }
        }
        
        public void ExecCommand()
        {
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
