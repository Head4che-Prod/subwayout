using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ChairsManager : MonoBehaviour
{
    private NetworkVariable<bool> _isUp= new NetworkVariable<bool>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject child in this.transform)
        {
            NetworkVariable<bool> mybool = child.GetComponent<NetworkVariable<bool>>();
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
}
