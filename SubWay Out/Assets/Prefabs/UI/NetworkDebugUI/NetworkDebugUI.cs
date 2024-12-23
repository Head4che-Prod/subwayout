using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class NetworkDebugUI : MonoBehaviour
{
    [SerializeField] Button ConnectClientButton;
    [SerializeField] Button ConnectHostButton;

    private void Start()
    {
        Button connectHostButton = ConnectHostButton.GetComponent<Button>();
        connectHostButton.onClick.AddListener(ConnectHost);

        Button connectClientButton = ConnectClientButton.GetComponent<Button>();
        connectClientButton.interactable = false;
        connectClientButton.onClick.AddListener(ConnectClient);
    }

    public void ConnectHost()
    {
        NetworkManager.Singleton.StartHost();
        ConnectHostButton.GetComponent<Button>().interactable = false;
        Debug.Log("Host connected successfully");
    }

    public void ConnectClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client connected successfully");
    }
}