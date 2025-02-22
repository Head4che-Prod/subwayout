using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DebugConsole : MonoBehaviour
{
    public static DebugConsole singleton;
    [SerializeField] Text displayText;
    [SerializeField] Selectable InputField;
    private static Dictionary<string, Action> commands;
    private static bool isActivated = false;

    void Awake()
    {
        if (singleton != null)
            DestroyImmediate(gameObject);
        else
        {
            singleton = this;

            commands = new Dictionary<string, Action>();
            commands.Add("sayHello", () => Log("Hello, world !"));
        }

        gameObject.transform.GetChild(0).gameObject.SetActive(isActivated);
    }

    public void focusOnConsole(InputAction.CallbackContext context) {
        if (context.performed)
            InputField.Select();
    }

    public void onModeActivationCommand(InputAction.CallbackContext context)
    {
        if (context.performed) {
            isActivated = !isActivated;
            gameObject.transform.GetChild(0).gameObject.SetActive(isActivated);
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
        commands[name] = func;
    }

    public void ExecCommand(string msg)
    {
        commands.GetValueOrDefault(msg, () => LogError("Command doesn't exist"))();
        (InputField as TMPro.TMP_InputField).text = "";
        EventSystem.current.SetSelectedGameObject(null);
    }
}
