using Unity.Netcode;
using UnityEngine;

public class AutoConnectPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       NetworkManager.Singleton.StartHost(); 
    }

}
