using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerObjectPick : MonoBehaviour
    {
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private Transform objectGrabPointTransform;
        [SerializeField] private float reach;

        private ObjectGrabbable _objectGrabbable;
        private void Update()
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * reach, Color.blue);
            if (InputSystem.actions.FindAction("Player/Interact").WasPressedThisFrame())
            {
                if (_objectGrabbable is null) // No item in hand
                {
                    if (Physics.Raycast(playerCamera.transform.position,
                            playerCamera.transform.forward, out RaycastHit raycastHit, reach))
                    {
                        if (raycastHit.transform.TryGetComponent(out ObjectGrabbable objGrabbable) && objGrabbable.Grabbable())
                        {
                            _objectGrabbable = objGrabbable;
                            _objectGrabbable.Grab(objectGrabPointTransform);
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