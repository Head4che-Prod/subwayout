using System;
using Prefabs.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerObject : NetworkBehaviour
    {
        [NonSerialized] public int CurrentSkin = 0;
        public PlayerInputManager InputManager {get; private set;}
        public PlayerMovement Movement {get; private set;}
        public PlayerInteract Interaction {get; private set;}
        public Rigidbody Rigidbody {get; private set;}
        public PlayerInput Input {get; private set;}
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
            // foreach (PlayerObject player in GameObject.FindGameObjectsWithTag)
            // {
                
            // }
            Transform models = transform.Find("Character");
            for (int i = 1; i < models.childCount; i++)
            {
                models.GetChild(i).gameObject.SetActive(PlayerSelection.CurrentPlayer + 1 == i);
            }

            if (!IsLocalPlayer) {
                transform.Find("UI").gameObject.SetActive(false);
                transform.Find("Canvas").gameObject.SetActive(false);
                transform.Find("CameraPos").gameObject.SetActive(false);
                transform.Find("CameraHolder").gameObject.SetActive(false);
            }
        }
    }
}