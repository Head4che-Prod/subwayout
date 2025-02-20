using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactive
{
    public class Grabbable : Interactive
    {
        [SerializeField] private Transform objectGrabPointTransform;
        private ObjectGrabbable _objectGrabbable;
        public Grabbable() : base("Player/Grab") {}
        public override void OnPress(InputAction.CallbackContext context)
        {
        }

        public override void OnPress(InputAction.CallbackContext context, RaycastHit raycastHit)
        {
            if (_objectGrabbable is null) // No item in hand and make sure that _objectGrabbable is not null
            {
                // Debug.Log(raycastHit.collider.gameObject.name);
                if (
                    raycastHit.transform.TryGetComponent(out ObjectGrabbable objGrabbable) && 
                    objGrabbable.Grabbable
                    )
                {
                    _objectGrabbable = objGrabbable;
                    _objectGrabbable.Grab(objectGrabPointTransform, playerCamera.transform);
                }
            }
            else
            {
                _objectGrabbable.Drop();
                _objectGrabbable = null;
            }
        }

        public override void OnRelease(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
    }
}