using System;
using System.Collections.Generic;
using Prefabs.Puzzles.FoldingSeats;
using Unity.Netcode;
using UnityEngine;

public class ChairsManager : MonoBehaviour
{
    private NetworkVariable<bool>[] _chairsBool = new NetworkVariable<bool>[24];
    private static ChairsManager _singleton;

    public static ChairsManager Singleton
    {
        get
        {
            return _singleton;
        }
    }

    void Start()
    {
        _singleton = this;
        for (int i = 0; i < 24; i++)
        {
            _chairsBool[i] =   transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<SingleChair>().isUp; //GetChild(0) 2 times because we search for "bottom" and the Find("bottom" doesn't work
        }
    }

    public void CheckChairs()
    {
        if (_chairsBool[0].Value && _chairsBool[1].Value
                                 && _chairsBool[6].Value && _chairsBool[7].Value 
                                 && _chairsBool[8].Value && _chairsBool[12].Value 
                                 && _chairsBool[17].Value && _chairsBool[19].Value 
                                 && _chairsBool[22].Value)
        {
            Debug.Log("Win Chairs");
        }
        
    }
}
