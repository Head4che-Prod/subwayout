using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Data;

public class NetworkDebugUI : MonoBehaviour
{
    [SerializeField] Button ConnectClientButton;
    [SerializeField] Button ConnectHostButton;

    private void Start()
    {
        Button connectHostButton = ConnectHostButton.GetComponent<Button>();
        connectHostButton.onClick.AddListener(ConnectHost);

        Button connectClientButton = ConnectClientButton.GetComponent<Button>();
        connectClientButton.onClick.AddListener(ConnectClient);

        NetworkManager.Singleton.OnClientDisconnectCallback += ResetButtons;   // Resets button on disconnect
    }

    public void ConnectHost()
    {
        NetworkManager.Singleton.StartHost();
        DisactivateButtons();
        Debug.Log("Host connected successfully");
    }

    public void ConnectClient()
    {
        NetworkManager.Singleton.StartClient();
        DisactivateButtons();
        Debug.Log("Client connected successfully");
    }

    private void DisactivateButtons()
    {
        ConnectHostButton.GetComponent<Button>().interactable = false;
        ConnectClientButton.GetComponent<Button>().interactable = false;
    }

    // Will need to be changed once the UI is player-specific
    private void ResetButtons(ulong ClientID)
    {
        ConnectHostButton.GetComponent<Button>().interactable = true;
        ConnectClientButton.GetComponent<Button>().interactable = true;
    }
}