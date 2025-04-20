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

        }

        public override void OnNetworkSpawn()
        {
            if (IsLocalPlayer)
                PlayerSkins.Value.Add(NetworkManager.Singleton.LocalClientId, (byte)(PlayerSelection.CurrentPlayerSkin + 1));
        }

        public static void SetSkins()
        {
            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                Transform models = client.Value.PlayerObject.transform.Find("Character");
                byte skin = PlayerSkins.Value[client.Key];
                for (int i = 1; i < models.childCount; i++)     // First child in unused
                {
                    models.GetChild(i).gameObject.SetActive(skin == i);
                }
            }
        }
    }
}