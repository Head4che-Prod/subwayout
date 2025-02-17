using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactive

{
    public abstract class Interactive : NetworkBehaviour
    {
        [SerializeField] protected GameObject playerCamera; // Defined for inherited class
        [SerializeField] protected InputAction inputKey;
        [SerializeField] private float reach = 3;

        /// <summary>
        /// Abstract of all objects that the player can interact with.
        /// </summary>
        /// <param name="actionName"> Name of the action, defined in InputSystem.</param>
        public Interactive(string actionName)
        {
            inputKey = InputSystem.actions.FindAction(actionName);
        }
        public void Update()
        {
            if (inputKey.WasPressedThisFrame()) OnPress();
            else if(inputKey.WasReleasedThisFrame()) OnRelease();
        }

        /// <summary>
        /// This function is called when the key is pressed.
        /// </summary>
        public abstract void OnPress();
        
        /// <summary>
        /// This function is called when the key is released.
        /// </summary>
        public abstract void OnRelease();

    }
}