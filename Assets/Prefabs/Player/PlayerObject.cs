using Prefabs.GameManagers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerObject : NetworkBehaviour
    {
        public PlayerInputManager InputManager { get; private set; }
        public PlayerMovement Movement { get; private set; }
        public PlayerInteract Interaction { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public PlayerInput Input { get; private set; }
        public Camera playerCamera;
        public Transform grabPointTransform;
        public GameObject playerCharacter;
        public GameObject debugConsolePrefab;
        public static PlayerObject LocalPlayer { get; private set; }
        
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
                InputManager.enabled = false;
                Movement.enabled = false;
                Interaction.enabled = false;
                Input.enabled = false;
            }
            else
            {
                LocalPlayer = this;
                Instantiate(debugConsolePrefab, transform.Find("UI"));
            }
        }

        public override void OnDestroy()
        {
            if (GrabbedObjectManager.Exists) GrabbedObjectManager.ForgetPlayer(this);
            base.OnDestroy();
        }
    }
}