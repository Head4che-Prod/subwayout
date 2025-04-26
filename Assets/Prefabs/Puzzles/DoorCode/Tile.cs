using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Tile : Objects.ObjectActionable
{

    [Rpc(SendTo.Server)]
    public void UpdateValueRpc()
    {
        value.Value = (byte)((value.Value + 1) % 10);
    }

    protected override void Action(PlayerObject player)
    {
        UpdateValueRpc();
    }

    private Text TextField;
    public NetworkVariable<byte> value = new NetworkVariable<byte>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
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
