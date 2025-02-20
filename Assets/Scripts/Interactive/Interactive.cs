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
            inputKey.performed += HandlePress;
            inputKey.canceled += OnRelease;
        }

        public void HandlePress(InputAction.CallbackContext context)
        {
            OnPress(context);
            if (Physics.Raycast(playerCamera.transform.position,
                    playerCamera.transform.forward, out RaycastHit raycastHit, reach))
                OnPress(context, raycastHit);
        }

        /// <summary>
        /// This function is called when the key is pressed.
        /// </summary>
        public abstract void OnPress(InputAction.CallbackContext context);
        
        /// <summary>
        /// This function is called when the key is pressed and object reached.
        /// </summary>
        public abstract void OnPress(InputAction.CallbackContext context, RaycastHit raycastHit);
        
        /// <summary>
        /// This function is called when the key is released.
        /// </summary>
        public abstract void OnRelease(InputAction.CallbackContext context);

    }
}