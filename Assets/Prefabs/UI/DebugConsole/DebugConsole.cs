using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DebugConsole : MonoBehaviour
{
    public static DebugConsole singleton;
    [SerializeField] Text displayText;
    [SerializeField] Selectable InputField;
    private static Dictionary<string, Action> commands;

    void Awake()
    {
        if (singleton != null)
            DestroyImmediate(gameObject);
        else
            singleton = this;

        commands = new Dictionary<string, Action>();
        commands.Add("sayHello", () => Log("Hello, world !"));
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            InputField.Select();
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
