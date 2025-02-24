using System;
using Objects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerInteract : NetworkBehaviour
    {
        [Header("Player")]
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private Transform objectGrabPointTransform;
        [SerializeField] private float reach;
        
        private ObjectGrabbable _grabbedObject;
        private InputAction _actionInput;
        private InputAction _grabInput;
        
        private void Start()
        {
            _actionInput = InputSystem.actions.FindAction("Player/Interact");
            _grabInput = InputSystem.actions.FindAction("Player/Grab");

            _actionInput.performed += HandlePress;
            _actionInput.canceled += HandleRelease;
            _grabInput.performed += HandlePress;
            _grabInput.canceled += HandleRelease;
        }
        
        /// <summary>
        /// This method handle button press.
        /// </summary>
        /// <param name="context"><see cref="InputAction"/>'s <see cref="InputAction.CallbackContext"/> of the press</param>
        private void HandlePress(InputAction.CallbackContext context)
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * reach, Color.blue);
            RaycastHit[] hits = new RaycastHit[2];
            Physics.RaycastNonAlloc(playerCamera.transform.position, playerCamera.transform.forward, hits, reach);
            
            if (hits[0].transform.TryGetComponent(out ObjectInteractive interactive))
            {
                // Action an object
                if (context.action.id == _actionInput.id && interactive is ObjectActionable objActionable) 
                {   
                    objActionable.HandleAction();
                }
                
                // Grab an object
                else if (context.action.id == _grabInput.id &&
                         interactive is ObjectGrabbable { Grabbable: true } objGrabbable &&
                         _grabbedObject is null)
                {
                    _grabbedObject = objGrabbable;
                    _grabbedObject.Grab(objectGrabPointTransform, playerCamera.transform);
                    return;
                }
            } 
            
            if (_grabbedObject is not null && context.action.id == _grabInput.id)
            {
                // Place an object
                if (
                    hits.Length >= 2 && 
                    hits[1].transform.TryGetComponent(out ObjectPlaceholder placeholder) &&
                    placeholder.Free &&
                    _grabbedObject.ConvertActionableType == placeholder.actionableType
                )
                {
                    _grabbedObject.ToActionable(placeholder);
                    _grabbedObject = null;
                }
            
                // Drop an object
                else
                {
                    _grabbedObject.Drop();
                    _grabbedObject = null;
                }
                
            }
        }

        /// <summary>
        /// This method handle button release.
        /// </summary>
        /// <param name="context"><see cref="InputAction"/>'s <see cref="InputAction.CallbackContext"/> of the release</param>
        private void HandleRelease(InputAction.CallbackContext context)
        {
        }
    }
}