using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Prefabs.UI.DebugConsole
{
    public class DebugConsole : MonoBehaviour
    {
        public static DebugConsole Singleton;
        [SerializeField] Text displayText;
        [FormerlySerializedAs("InputField")] [SerializeField] Selectable inputField;
        private static Dictionary<string, Action> _commands;
        private static bool _isActivated = false;

        void Awake()
        {
            if (Singleton != null)
                DestroyImmediate(gameObject);
            else
            {
                Singleton = this;

                _commands = new Dictionary<string, Action>();
                _commands.Add("sayHello", () => Log("Hello, world !"));
                _commands.Add("inputMode", () => Log(EventSystem.current.GetComponent<PlayerInput>().currentActionMap.name));
            }

            gameObject.transform.GetChild(0).gameObject.SetActive(_isActivated);
        }

        public void focusOnConsole(InputAction.CallbackContext context) {
            if (context.performed && this.gameObject.activeInHierarchy){
                inputField.Select();
                EventSystem.current.GetComponent<PlayerInput>().SwitchCurrentActionMap("Debug");
            }
        }

        public void OnModeActivationCommand(InputAction.CallbackContext context)
        {
            if (context.performed && this.gameObject.activeInHierarchy) {
                _isActivated = !_isActivated;
                gameObject.transform.GetChild(0).gameObject.SetActive(_isActivated);
            }
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

        public static void AddCommand(string name, Action func)
        {
            _commands[name] = func;
        }

        public void ExecCommand(string msg)
        {
            _commands.GetValueOrDefault(msg, () => LogError("Command doesn't exist"))();
            EventSystem.current.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
            ((TMP_InputField)inputField).text = "";
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
