using UnityEngine.InputSystem;

namespace Interactive
{
    public class Grabbable : Interactive
    {
        public Grabbable() : base("Player/Grab") {}
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