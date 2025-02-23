using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerObject : MonoBehaviour
    {
        public PlayerInputManager InputManager {get; private set;}
        public PlayerMovement Movement {get; private set;}
        public PlayerObjectPick ObjectPick {get; private set;}
        public Rigidbody Rigidbody {get; private set;}
        public PlayerInput Input {get; private set;}
        public Camera Camera;
        public Transform GrabPointTransform;

        public void Awake()
        {
            InputManager = GetComponent<PlayerInputManager>();
            Movement = GetComponent<PlayerMovement>();
            ObjectPick = GetComponent<PlayerObjectPick>();
            Rigidbody = GetComponent<Rigidbody>();
            Input = GetComponent<PlayerInput>();
        }
    }
}