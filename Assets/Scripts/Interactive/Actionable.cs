using System;
using UnityEngine.InputSystem;

namespace Interactive
{
    public class Actionable : Interactive
    {
        public Actionable() : base("Player/Interact") {}

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