using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactive
{
    public class Actionable : Interactive
    {
        [SerializeField] private Transform objectGrabPointTransform;
        private ObjectGrabbable _objectGrabbable;
        public Actionable() : base("Player/Interact") {}

        public override void OnPress(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public override void OnPress(InputAction.CallbackContext context, RaycastHit raycastHit)
        {
            if (raycastHit.transform.TryGetComponent(out ObjectGrabbable objGrabbable) && objGrabbable.Grabbable)
            {
                _objectGrabbable = objGrabbable;
                _objectGrabbable.Grab(objectGrabPointTransform, playerCamera.transform);
            }
        }

        public override void OnRelease(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
    }
}