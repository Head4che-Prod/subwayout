using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class CodeGenerator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_Object;
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
                foreach (string part in ipAdress)
                {
                    int n = int.Parse(part);
                    textInput += $"{chars[n % chars.Length]}{chars[n / chars.Length]}";
                }
            }
        }

        m_Object.text = textInput;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
