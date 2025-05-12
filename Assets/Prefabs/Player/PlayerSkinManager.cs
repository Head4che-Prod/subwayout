using PlayerSelectionMenu;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Player
{
    public class PlayerSkinManager : NetworkBehaviour
    {
        public void Start()
        {
            GetPlayerSkinRpc();
        }

        [Rpc(SendTo.Owner)]
        private void GetPlayerSkinRpc()
        {
            SetPlayerSkinRpc(PlayerSelection.CurrentPlayerSkin);
        }

        [Rpc(SendTo.Everyone)]
        private void SetPlayerSkinRpc(byte skinIndex)
        {
            Transform models = transform.Find("Character");
            for (int i = 0; i < models.childCount - 1; i++) // First child in unused
                models.GetChild(i + 1).gameObject.SetActive(skinIndex == i);
        }
    }
}