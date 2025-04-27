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
        /// <summary>
        /// Dictionary matching the player's client ID to their skin ID.
        /// </summary>
        private static readonly Dictionary<ulong, byte> PlayerSkins = new Dictionary<ulong, byte>();

        /// <summary>
        /// Internal singleton instance of the class, used to call <see cref="ApplySkinsInstruction"/>.
        /// </summary>
        private static PlayerSkinManager _singleton;

        /// <summary>
        /// Accessor and setter for the <see cref="_singleton"/>.
        /// </summary>
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
        
        /// <summary>
        /// If <paramref name="clientId"/> matches the client's ID, requests the server to log the client's skin. Called when a player connects to the network.
        /// </summary>
        /// <param name="clientId">The ID of the player whose skin is to be set.</param>
        private void SetPlayerSkin(ulong clientId)
        {
            if (NetworkManager.Singleton.LocalClientId == clientId)
                SetSkinServerRpc(NetworkManager.Singleton.LocalClientId, (byte)(PlayerSelection.CurrentPlayerSkin + 1));
        }
        
        /// <summary>
        /// Logs a player's skin to the server's <see cref="PlayerSkins"/> dictionary.
        /// </summary>
        /// <param name="clientId">The ID of the player whose skin is to be set.</param>
        /// <param name="skinId">The skin ID to set for the player.</param>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void SetSkinServerRpc(ulong clientId, byte skinId)
        {
            PlayerSkins[clientId] = skinId;
            Debug.Log($"Logging clients {clientId}'s skin.");
        }

        /// <summary>
        /// Requests all clients to apply the skins in <see cref="PlayerSkins"/>. Must be called by the server.
        /// </summary>
        public void ApplySkinsInstruction() => ApplySkinsInstructionRpc(new DictUlongByteWrapper(PlayerSkins));
        /// <summary>
        /// Resets the internal skin registry upon pressing "Start".
        /// </summary>
        public void ResetSkinRegistry() => PlayerSkins.Clear();
        
        /// <summary>
        /// Applies the skins on all the <see cref="PlayerObject"/> clones.
        /// </summary>
        /// <param name="playerSkins">Wrapper containing the skins to apply to the ID of each clone.</param>
        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void ApplySkinsInstructionRpc(DictUlongByteWrapper playerSkins)
        {
            Debug.Log("Received request to apply skins.");
            
            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                Transform models = client.Value.PlayerObject.transform.Find("Character");
                byte skin = playerSkins.Dictionary.GetValueOrDefault(client.Key, (byte)1);
                for (int i = 1; i < models.childCount; i++)     // First child in unused
                {
                    models.GetChild(i).gameObject.SetActive(skin == i);
                }
                Debug.Log($"Applied skin of client {client.Key} on client {NetworkManager.Singleton.LocalClientId}.");
            }
        }
    }
}