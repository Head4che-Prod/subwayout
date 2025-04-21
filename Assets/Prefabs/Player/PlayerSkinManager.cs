using System;
using System.Collections.Generic;
using Prefabs.UI;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Player
{
    public class PlayerSkinManager : NetworkBehaviour
    {
        [NonSerialized] public static NetworkVariable<Dictionary<ulong, byte>> PlayerSkins =
            new NetworkVariable<Dictionary<ulong, byte>>(new Dictionary<ulong, byte>());

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
            if (PlayerSkins.Value.TryAdd(clientId, skinId))
                Debug.Log($"Logging clients {clientId}'s skin.");
        }

        public void ApplySkinsInstruction() => ApplySkinsInstructionRpc();
        
        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void ApplySkinsInstructionRpc()
        {
            Debug.Log("Received request to change skin.");
            ApplySkins();
        }
        
        private static void ApplySkins()
        {
            Debug.Log("Applying skins...");
            foreach (KeyValuePair<ulong, byte> elm in PlayerSkins.Value)
                Debug.Log($"Dict: {elm.Key} - {elm.Value}");
            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                Transform models = client.Value.PlayerObject.transform.Find("Character");
                byte skin = PlayerSkins.Value[client.Key];
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