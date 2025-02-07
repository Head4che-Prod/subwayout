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
    private static bool isActivated = false;
    private static int touchCount = 0;
    private static float lastTouch = 0;

    void Awake()
    {
        if (singleton != null)
            DestroyImmediate(gameObject);
        else
        {
            singleton = this;

            commands = new Dictionary<string, Action>();
            commands.Add("sayHello", () => Log("Hello, world !"));

            touchCount = 0;
            lastTouch = 0;
        }

        gameObject.transform.GetChild(0).gameObject.SetActive(isActivated);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            InputField.Select();
        }

        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            if (Time.time - lastTouch < .5)
            {
                touchCount++;
            }
            else
            {
                touchCount = 0;
            }
            lastTouch = Time.time;
            if (touchCount >= 5)
            {
                isActivated = !isActivated;
                touchCount = 0;
                gameObject.transform.GetChild(0).gameObject.SetActive(isActivated);
            }
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
