using UnityEngine;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiGrabbable : ObjectGrabbable
    {
        private static bool _useable = true;
        //public override bool Grabbable => _useable && IsGrabbable;  // Checks whether the hanoi towers are already in use.

        public override void Grab(Transform objectGrabPointTransform)
        {
            base.Grab(objectGrabPointTransform);
            
            _useable = false;
        }
        public override void Drop()
        {
            base.Drop();
            
            HanoiCollider.ResetBall(Rb);
            _useable = true;
        }
    }
}