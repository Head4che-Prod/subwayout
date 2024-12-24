using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkDebugUI : NetworkBehaviour
{
    [SerializeField] Button ConnectClientButton;
    [SerializeField] Button ConnectHostButton;

    private NetworkVariable<bool> buttonsActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Owner);


    public override void OnNetworkSpawn()
    {
        buttonsActive.OnValueChanged += (bool _, bool _) => ManageButtons();
        NetworkManager.Singleton.OnConnectionEvent += (NetworkManager _, ConnectionEventData eventData) => buttonsActive.Value = eventData.ClientId == NetworkObjectId;
    }


    private void Start()
    {
        Button connectHostButton = ConnectHostButton.GetComponent<Button>();
        connectHostButton.onClick.AddListener(ConnectHost);

        Button connectClientButton = ConnectClientButton.GetComponent<Button>();
        connectClientButton.onClick.AddListener(ConnectClient);

        buttonsActive.Value = true;
    }

    public void ConnectHost()
    {
        NetworkManager.Singleton.StartHost();
        buttonsActive.Value = false;
        ManageButtons();
        Debug.Log("Host connected successfully");
    }

    public void ConnectClient()
    {
        NetworkManager.Singleton.StartClient();
        buttonsActive.Value = false;
        ManageButtons();
        Debug.Log("Client connected successfully");
    }

    private void ManageButtons()
    {
        ConnectHostButton.GetComponent<Button>().interactable = buttonsActive.Value;
        ConnectClientButton.GetComponent<Button>().interactable = buttonsActive.Value;
    }
}