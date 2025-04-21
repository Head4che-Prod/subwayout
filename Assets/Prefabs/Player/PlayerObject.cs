using System;
using System.Collections.Generic;
using Prefabs.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerObject : NetworkBehaviour
    {
        [NonSerialized] public static NetworkVariable<Dictionary<ulong, byte>> PlayerSkins =
            new NetworkVariable<Dictionary<ulong, byte>>(new Dictionary<ulong, byte>());
        public PlayerInputManager InputManager { get; private set; }
        public PlayerMovement Movement { get; private set; }
        public PlayerInteract Interaction { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public PlayerInput Input { get; private set; }
        public Camera playerCamera;
        public Transform grabPointTransform;
        public GameObject playerCharacter;

        public void Awake()
        {
            InputManager = GetComponent<PlayerInputManager>();
            Movement = GetComponent<PlayerMovement>();
            Interaction = GetComponent<PlayerInteract>();
            Rigidbody = GetComponent<Rigidbody>();
            Input = GetComponent<PlayerInput>();
        }

        public void Start()
        {
            if (!IsLocalPlayer)
            {
                transform.Find("UI").gameObject.SetActive(false);
                transform.Find("Canvas").gameObject.SetActive(false);
                transform.Find("CameraPos").gameObject.SetActive(false);
                transform.Find("CameraHolder").gameObject.SetActive(false);
            }

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
        
        public static void ApplySkins()
        {
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