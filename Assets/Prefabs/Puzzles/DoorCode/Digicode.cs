using System;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Digicode : MonoBehaviour
{
    private const int password = 7642;
    private static int EnteredPassword = 0;
    private static bool CanDoorOpen { get => EnteredPassword == password; }
    public void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            Tile tile = transform.GetChild(i).GetComponent<Tile>();
            tile.Coef = (new[] { 1000, 100, 10, 1 })[i];
            tile.value.OnValueChanged += (oldVal, newVal) =>
            {
                EnteredPassword += (newVal - oldVal) * tile.Coef;
            };
        }
    }
}
