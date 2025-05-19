using Objects;
using Prefabs.GameManagers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Prefabs.Player
{
    public class PlayerObject : NetworkBehaviour, IResettablePosition
    {
        public PlayerInputManager InputManager { get; private set; }
        public PlayerMovement Movement { get; private set; }
        public PlayerInteract Interaction { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public PlayerInput Input { get; private set; }
        public Camera playerCamera;
        public Transform grabPointTransform;
        public GameObject playerCharacter;
        public static bool DisplayHints = true;
        public GameObject debugConsolePrefab;
        
        
        public Vector3 InitialPosition { get; set; }
        public Quaternion InitialRotation { get; set; }
        
        public static PlayerObject LocalPlayer { get; private set; }
        
        public void Awake()
        {
            InputManager = GetComponent<PlayerInputManager>();
            Movement = GetComponent<PlayerMovement>();
            Interaction = GetComponent<PlayerInteract>();
            Rigidbody = GetComponent<Rigidbody>();
            Input = GetComponent<PlayerInput>();
            InitialPosition = Rigidbody.position;
            InitialRotation = Rigidbody.rotation;
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
                SceneManager.activeSceneChanged += SetSpawnPos;
                transform.Find("Canvas").GetChild(1).gameObject.SetActive(DisplayHints);
                LocalPlayer = this;
                Instantiate(debugConsolePrefab, transform.Find("UI"));
                ObjectHighlightManager.Init();
            }
        }

        private void SetSpawnPos(Scene prev, Scene next) 
        {
            if (next.name == "DemoScene") {
                if (IsHost)
                {
                    Rigidbody.position = new Vector3(2, .465f, -3.3f);
                    Rigidbody.rotation = new Quaternion(0, 0, 0, 1);
                }
                else
                {
                    Rigidbody.position = new Vector3(2, .465f, -.95f);
                    Rigidbody.rotation = new Quaternion(0, 1, 0, 0);
                }
            }
            InitialPosition = Rigidbody.position;
            InitialRotation = Rigidbody.rotation;
        }
        
        public void ResetPosition() => ResetPositionClientRpc();

        [Rpc(SendTo.ClientsAndHost)]
        private void ResetPositionClientRpc()
        {
            Rigidbody.position = InitialPosition;
            Rigidbody.rotation = InitialRotation;
        }

        public override void OnDestroy()
        {
            SceneManager.activeSceneChanged -= SetSpawnPos;
            if (GrabbedObjectManager.Exists) GrabbedObjectManager.ForgetPlayer(this);
            base.OnDestroy();
        }
        
    }
}