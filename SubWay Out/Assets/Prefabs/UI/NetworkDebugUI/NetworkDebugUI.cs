using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkDebugUI : MonoBehaviour
{
    // Connect buttons
    [SerializeField] Button connectClientButton;
    [SerializeField] Button connectHostButton;
    
    private void Start()
    {
        connectHostButton.GetComponent<Button>().onClick.AddListener(ConnectHost);
        connectClientButton.GetComponent<Button>().onClick.AddListener(ConnectClient);
    }
    private void ConnectHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host connected successfully");
    }

    private void ConnectClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client connected successfully");
    }
}