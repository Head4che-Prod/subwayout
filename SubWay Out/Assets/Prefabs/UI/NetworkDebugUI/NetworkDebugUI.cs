using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Serialization;

public class NetworkDebugUI : NetworkBehaviour
{
    // Connect buttons
    [SerializeField] Button connectClientButton;
    [SerializeField] Button connectHostButton;

    // Should buttons be active? (per client)
    private NetworkVariable<bool> _buttonsActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Owner); // initialization -> buttonsOn


    public override void OnNetworkSpawn()
    {
        // Every time _buttonActive changes, activate of deactivate the buttons accordingly
        _buttonsActive.OnValueChanged += (bool _, bool _) => ManageButtons();
        
        // On global disconnect, reactivate buttons
        NetworkManager.Singleton.OnServerStopped += (bool _) => _buttonsActive.Value = true; // serverStop -> buttonsOn
        
        NetworkManager.Singleton.OnConnectionEvent += (NetworkManager _, ConnectionEventData eventData) => HandleClientDisconnect(eventData);
    }

    private void HandleClientDisconnect(ConnectionEventData eventData)
    {
        if (eventData.EventType == ConnectionEvent.ClientDisconnected)
        {
            Debug.Log($"Client disconnected with ID {eventData.ClientId}. Current id is: {OwnerClientId}");
        }
    }
    
    private void Start()
    {
        connectHostButton.GetComponent<Button>().onClick.AddListener(ConnectHost);
        connectClientButton.GetComponent<Button>().onClick.AddListener(ConnectClient);
    }

    private void ConnectHost()
    {
        NetworkManager.Singleton.StartHost();
        
        // Must be called here because network has not spawned in yet
        ManageButtons(false); // serverStart -> buttonsOff
        
        Debug.Log("Host connected successfully");
    }

    private void ConnectClient()
    {
        NetworkManager.Singleton.StartClient();
        
        // Must be called here because network has not spawned in yet
        ManageButtons(false); // clientStart -> buttonsOff
        
        Debug.Log("Client connected successfully");
    }

    private void ManageButtons(bool? manualSetter = null)
    {
        // Sets buttons active to _buttonsActive.Value, except if manualSetter is defined
        connectHostButton.GetComponent<Button>().interactable = manualSetter ?? _buttonsActive.Value;
        connectClientButton.GetComponent<Button>().interactable = manualSetter ?? _buttonsActive.Value;
    }
}