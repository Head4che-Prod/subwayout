using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkDebugUI : MonoBehaviour
{
    [SerializeField] private Button ConnectHostButton;
    [SerializeField] private Button ConnectClientButton;

    private void Awake()
    {
        Debug.LogWarning("Button connected");
        ConnectHostButton.onClick.AddListener(ConnectHost);
    }

    private void ConnectHost()
    {
        Debug.LogWarning("Clicked button");
        NetworkManager.Singleton.StartHost();
    }
}