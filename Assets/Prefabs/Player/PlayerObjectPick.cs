using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerObjectPick : NetworkBehaviour
    {
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private Transform objectGrabPointTransform;
        [SerializeField] private float reach;

        private ObjectGrabbable _objectGrabbable;
        private InputAction _interactInput;

        private void Start()
        {
            _interactInput = InputSystem.actions.FindAction("Player/Interact");
        }

        private void Update()
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * reach, Color.blue);
            if (_interactInput.WasPressedThisFrame())
            {
                if (_objectGrabbable is null) // No item in hand
                {
                    if (Physics.Raycast(playerCamera.transform.position,
                            playerCamera.transform.forward, out RaycastHit raycastHit, reach))
                    {
                        // Debug.Log(raycastHit.collider.gameObject.name);
                        if (raycastHit.transform.TryGetComponent(out ObjectGrabbable objGrabbable) && objGrabbable.Grabbable)
                        {
                            _objectGrabbable = objGrabbable;
                            _objectGrabbable.Grab(objectGrabPointTransform, playerCamera.transform);
                        }
                    }
                }
                else
                {
                    _objectGrabbable.Drop();
                    _objectGrabbable = null;
                }
            }
        }
    }
}