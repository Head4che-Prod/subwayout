using System;
using Objects;
using Prefabs.Player;
using Prefabs.Puzzles.Hanoi.Debugs;
using UnityEngine;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiGrabbable : ObjectGrabbable
    {
        
        private static bool _useable = true;
        
        public override bool Grabbable => _useable && IsGrabbable.Value;  // Checks whether the hanoi towers are already in use.
        public override void Grab(PlayerObject player)
        {
            base.Grab(player);
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
            if (Physics.Raycast(Owner.playerCamera.transform.position, Owner.playerCamera.transform.forward,
                    out RaycastHit hit, 12f, 3))
            {
                if (HanoiTowers.Instance.IsInDebugMode)
                {
                    MovementVector.Instance.SetPosition(0, transform.position);
                    MovementVector.Instance.SetPosition(1, hit.point);
                }
                
                return hit.point - transform.position;
            }

            Drop();
            return Vector3.zero;
        }
    }
}