using System;
using Prefabs.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerObject : MonoBehaviour
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
            Transform models = transform.Find("Character");
            for (int i = 1; i < models.childCount; i++)
            {
                models.GetChild(i).gameObject.SetActive(PlayerSelection.CurrentPlayer + 1 == i);
            }
        }
    }
}