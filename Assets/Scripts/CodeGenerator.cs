using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class CodeGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string chars = "0123456789ABCDEFGHIJKLMNPQRSTUVWXYZ";
        string textInput = "Welcome to the station ";
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                string[] ipAdress = ip.ToString().Split('.');
                Debug.Log(ipAdress.Length);
                Debug.Log(ipAdress.ToString());
                foreach (string part in ipAdress)
                {
                    int n = int.Parse(part);
                    Debug.Log(n);
                    textInput += $"{chars[n % chars.Length]}{chars[n / chars.Length]}";
                }
                break;
            }
        }
        GameObject.FindGameObjectWithTag("MultiplayerTitle").GetComponent<TextMeshProUGUI>().text = textInput;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Close()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/HomeMenu", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
