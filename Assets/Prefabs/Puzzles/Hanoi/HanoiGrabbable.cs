using System;
using Prefabs.Puzzles.Hanoi.Debugs;
using UnityEngine;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiGrabbable : ObjectGrabbable
    {
        
        private static bool _useable = true;
        
        public override bool Grabbable => _useable && IsGrabbable;  // Checks whether the hanoi towers are already in use.
        public override void Grab(Transform objectGrabPointTransform, Transform playerCamera)
        {
            base.Grab(objectGrabPointTransform, playerCamera);
            //Debug.Log(HanoiCollider.DebugGrid());
            _useable = false;
        }
        public override void Drop()
        {
            base.Drop();
            
            // Debug.Log($"Let go of {name} at ({Rb.position.x}, {Rb.position.y}, {Rb.position.z})");
            HanoiCollider.ResetBall(transform);
            _useable = true;
        }

        public override Vector3 CalculateMovementForce()
        {
            Vector3 baseForce = base.CalculateMovementForce();
            Vector3 normalizedForce = baseForce - Vector3.Dot(baseForce, HanoiTowers.Instance.transform.forward) *
                HanoiTowers.Instance.transform.forward;
            MovementVector.Instance.SetPosition(0, Rb.position);
            MovementVector.Instance.SetPosition(1, Rb.position + normalizedForce);
            
            return normalizedForce;
        }
    }
}