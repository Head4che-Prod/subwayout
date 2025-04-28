using System;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine.UI;

namespace Prefabs.Puzzles.DoorCode
{
    public class Tile : Objects.ObjectActionable
    {
        /// <summary>
        /// Requests server to update the code's internal value.
        /// </summary>
        [Rpc(SendTo.Server)]
        private void UpdateValueRpc()
        {
            value.Value = (byte)((value.Value + 1) % 10);
        }

        protected override void Action(PlayerObject player)
        {
            if (Digicode.Active)
                UpdateValueRpc();
        }

        private Text _textField;
        /// <summary>
        /// The current value of the tile.
        /// </summary>
        public NetworkVariable<byte> value = new NetworkVariable<byte>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        /// <summary>
        /// Order of magnitude of the tile's value relative to the code.
        /// </summary>
        [NonSerialized] public int Order;

        void Start()
        {
            _textField = transform.GetChild(0).GetChild(0).GetComponent<Text>();
            _textField.text = $"{value.Value}";
            value.OnValueChanged += (prev, newVal) =>
            {
                _textField.text = $"{newVal}";
            };
        }
    }
}
