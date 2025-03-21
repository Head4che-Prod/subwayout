using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerObject : MonoBehaviour
    {
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
    }
}