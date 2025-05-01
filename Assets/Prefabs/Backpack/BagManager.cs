using System;
using System.Collections.Generic;
using Objects;
using Prefabs.Player.PlayerUI.DebugConsole;
using UnityEngine;


public class BagManager: ObjectActionable
{
    [SerializeField] private GameObject _bagOpen;
    [SerializeField] private GameObject _bagClose;
    [SerializeField] private List<GameObject> objectsInBag;


    private void Start()
    {
        DebugConsole.AddCommand("ImpulseTest", ImpulsionObjets);
    }

    protected override void Action()
    {
        Debug.Log("Action played");
        _bagOpen.SetActive(true);
        _bagClose.SetActive(false);
        foreach (GameObject obj in objectsInBag)
        {
            obj.SetActive(true);
        }
    }
    protected void ImpulsionObjets()
    {
        GameObject spawnedObj = Instantiate(objectsInBag[0], _bagOpen.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        spawnedObj.GetComponent<Rigidbody>().AddForce(new Vector3(10f, 10f, 10f), ForceMode.Impulse);
    }
    
    
}
