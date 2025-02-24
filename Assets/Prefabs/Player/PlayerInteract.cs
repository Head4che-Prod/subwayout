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
        
        private void HandlePress(InputAction.CallbackContext context)
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * reach, Color.blue);
            if (
                Physics.Raycast(
                    playerCamera.transform.position,
                    playerCamera.transform.forward,
                    out RaycastHit raycastHit, reach) &&
                raycastHit.transform.TryGetComponent(out ObjectInteractive interactive)
            )
            {
                if (context.action.id == _actionInput.id && interactive is ObjectActionable objActionable) 
                {   
                    objActionable.HandleAction();
                }
                else if (
                    context.action.id == _grabInput.id &&
                    interactive is ObjectGrabbable objGrabbable &&
                    objGrabbable.Grabbable &&
                    _grabbedObject is null
                )
                {
                    Debug.Log("Grab");
                    _grabbedObject = objGrabbable;
                    _grabbedObject.Grab(objectGrabPointTransform, playerCamera.transform);
                    return;
                }
            } 
            
            if (_grabbedObject is not null && context.action.id == _grabInput.id)
            {
                Debug.Log("Release");
                _grabbedObject.Drop();
                _grabbedObject = null;
            }
        }

        private void HandleRelease(InputAction.CallbackContext context)
        {
            
        }
    }
}