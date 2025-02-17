using System;
using UnityEngine.InputSystem;

namespace Interactive
{
    public class Actionable : Interactive
    {
        public Actionable() : base("Player/Interact") {}

        public override void OnPress()
        {
            throw new NotImplementedException();
        }

        public override void OnRelease()
        {
            throw new NotImplementedException();
        }
    }
}