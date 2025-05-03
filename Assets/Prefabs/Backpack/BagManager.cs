using System;
using System.Collections.Generic;
using Objects;
using Prefabs.Player.PlayerUI.DebugConsole;
using Unity.Netcode;
using UnityEngine;


public class BagManager: ObjectActionable
{
    [SerializeField] private GameObject _bagOpen;
    [SerializeField] private GameObject _bagClose;
    [SerializeField] private List<GameObject> objectsInBag;

    protected override void Action()
    {
        Debug.Log("Action played");
        _bagOpen.SetActive(true);
        _bagClose.SetActive(false);
        ImpulsionObjetsRpc();
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    protected void ImpulsionObjetsRpc()
    {
        foreach (GameObject obj in objectsInBag)
        {
            GameObject spawnedObj = Instantiate(obj, _bagOpen.transform.position + new Vector3(0, 1, 0),
                Quaternion.identity);
            spawnedObj.GetComponent<NetworkObject>().Spawn();
            spawnedObj.GetComponent<Rigidbody>().AddForce(new Vector3(-1.5f, 2f, 0f), ForceMode.VelocityChange);
        }
    }
    
    
}
