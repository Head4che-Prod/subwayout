using System;
using UnityEngine.InputSystem;

namespace Interactive
{
    public class Grabbable : Interactive
    {
        public Grabbable() : base("Player/Grab") {}
        public override void OnPress(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public override void OnRelease(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
    }
}