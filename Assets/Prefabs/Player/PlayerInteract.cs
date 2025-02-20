using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerInteract : NetworkBehaviour
    {
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
            if (
                Physics.Raycast(
                    playerCamera.transform.position, 
                    playerCamera.transform.forward, 
                    out RaycastHit raycastHit, reach)
                )
                if (context.action.id == _actionInput.id &&
                    raycastHit.transform.TryGetComponent(out ObjectActionnable objActionnable)
                    ) 
                {   
                    objActionnable.Action();
                }
                else if (
                    context.action.id == _grabInput.id &&
                    raycastHit.transform.TryGetComponent(out ObjectGrabbable objGrabbable) &&
                    objGrabbable.Grabbable
                    )
                {
                    if (_grabbedObject is null)
                    {
                        _grabbedObject = objGrabbable;
                        _grabbedObject.Grab(objectGrabPointTransform, playerCamera.transform);
                    }
                    else
                    {
                        _grabbedObject.Drop();
                        _grabbedObject = null;
                    }   
                }
        }

        private void HandleRelease(InputAction.CallbackContext context)
        {
            
        }
    }
}