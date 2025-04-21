using System;
using System.Collections.Generic;
using Prefabs.UI;
using Unity.Netcode;
using UnityEngine;
using Wrappers;

namespace Prefabs.Player
{
    public class PlayerSkinManager : NetworkBehaviour
    {
        private static readonly Dictionary<ulong, byte> PlayerSkins = new Dictionary<ulong, byte>();

        private static PlayerSkinManager _singleton;

        public static PlayerSkinManager Singleton
        {
            get
            {
                if (_singleton != null)
                    return _singleton; 
                Debug.LogWarning("PlayerSkinManager not initialized");
                return null;
            }
            private set
            {
                if (_singleton == null)
                    _singleton = value;
                else 
                    Debug.LogWarning("PlayerSkinManager already initialized");
            }
        }
        
        
        public void Start()
        {
            Singleton = this;
            NetworkManager.Singleton.OnClientConnectedCallback += SetPlayerSkin;
        }

        public override void OnNetworkSpawn()   // Do to wonky Network management on our side, this is called whenever a client connects.
        {
            SetPlayerSkin(NetworkManager.Singleton.LocalClientId);
        }
        
        private void SetPlayerSkin(ulong clientId)
        {
            if (NetworkManager.Singleton.LocalClientId == clientId)
                SetSkinServerRpc(NetworkManager.Singleton.LocalClientId, (byte)(PlayerSelection.CurrentPlayerSkin + 1));
        }
        
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void SetSkinServerRpc(ulong clientId, byte skinId)
        {
            if (PlayerSkins.TryAdd(clientId, skinId))
                Debug.Log($"Logging clients {clientId}'s skin.");
        }

        public void ApplySkinsInstruction() => ApplySkinsInstructionRpc(new DictUlongByteWrapper(PlayerSkins));
        
        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void ApplySkinsInstructionRpc(DictUlongByteWrapper playerSkins)
        {
            Debug.Log("Received request to change skin.");
            ApplySkins(playerSkins.Dictionary);
        }
        
        private static void ApplySkins(Dictionary<ulong, byte> playerSkins)
        {
            Debug.Log("Applying skins...");
            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                Transform models = client.Value.PlayerObject.transform.Find("Character");
                byte skin = playerSkins[client.Key];
                for (int i = 1; i < models.childCount; i++)     // First child in unused
                {
                    Debug.Log(models.GetChild(i).gameObject.name);
                    models.GetChild(i).gameObject.SetActive(skin == i);
                }
                Debug.Log($"Applied skin of client {client.Key} on client {NetworkManager.Singleton.LocalClientId}.");
            }
        }
    }
}