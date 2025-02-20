using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactive

{
    public abstract class Interactive : NetworkBehaviour
    {
        [SerializeField] protected InputAction inputKey;
        [SerializeField] protected GameObject playerCamera; // Defined for inherited class
        [SerializeField] private float reach = 3;

        /// <summary>
        /// Abstract of all objects that the player can interact with.
        /// </summary>
        /// <param name="actionName"> Name of the action, defined in InputSystem.</param>
        public Interactive(string actionName)
        {
            inputKey = InputSystem.actions.FindAction(actionName);
        }

        private void Start()
        {
            inputKey.performed += OnPress;
            inputKey.canceled += OnRelease;
        }

        /// <summary>
        /// This function is called when the key is pressed.
        /// </summary>
        public abstract void OnPress(InputAction.CallbackContext context);
        
        /// <summary>
        /// This function is called when the key is released.
        /// </summary>
        public abstract void OnRelease(InputAction.CallbackContext context);

    }
}