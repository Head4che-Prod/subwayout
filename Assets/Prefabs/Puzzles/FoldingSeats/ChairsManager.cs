using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ChairsManager : MonoBehaviour
{
    private NetworkVariable<bool> _isUp= new NetworkVariable<bool>();
    private Dictionary<GameObject, bool> _dictionaryChairsUporDown = new Dictionary<GameObject, bool>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject child in this.transform)
        {
            _dictionaryChairsUporDown.Add(child, child.GetComponent<bool>());
        }
    }


    private bool GetChairsUporDown(GameObject gameObject)
    {
        return gameObject.GetComponent<bool>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject child in this.transform)
        {
            _dictionaryChairsUporDown[child]= GetChairsUporDown(child);

            if (_dictionaryChairsUporDown[child])
            {
                _isUp.Value = true;
            }
        }
    }
}
