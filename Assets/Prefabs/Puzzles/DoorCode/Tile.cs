using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Tile : Objects.ObjectActionable
{
    protected override void Action(PlayerObject player)
    {
        value.Value = (byte)((value.Value + 1) % 10);
    }


    private Text TextField;
    public NetworkVariable<byte> value = new NetworkVariable<byte>(0);
    public int Coef { get; set; }

    void Start()
    {
        TextField = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        TextField.text = $"{value.Value}";
        value.OnValueChanged += (prev, newVal) =>
        {
            TextField.text = $"{newVal}";
        };
    }
}
