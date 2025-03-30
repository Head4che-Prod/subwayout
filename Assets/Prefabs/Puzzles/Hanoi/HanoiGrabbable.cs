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
                    out RaycastHit hit, 12f, 1000))  // Only let layer 3 pass
            {
                Vector3 direction = HanoiTowers.Instance.transform.InverseTransformVector(hit.point - transform.position);
                direction.y = 0;
                
                Vector3 hitPointLocal = HanoiTowers.Instance.transform.InverseTransformDirection(hit.point);
                Vector3 positionLocal = HanoiTowers.Instance.transform.InverseTransformDirection(transform.position);

                if (direction.magnitude > 0.2f)
                {
                    if (Math.Abs(hitPointLocal.x - positionLocal.x) >
                        Math.Abs(hitPointLocal.z - positionLocal.z) + 0.2f)
                        direction.z = 0;
                    else if (Math.Abs(hitPointLocal.z - positionLocal.z) >
                             Math.Abs(hitPointLocal.x - positionLocal.x) + 0.2f)
                        direction.x = 0;
                }
                
                direction = HanoiTowers.Instance.transform.InverseTransformVector(direction.normalized) * 0.1f;
                
                if (HanoiTowers.Instance.IsInDebugMode)
                {
                    MovementVector.Instance.SetPosition(0, transform.position);
                    MovementVector.Instance.SetPosition(1, direction);
                }
                
                return direction;
            }

            Drop();
            return Vector3.zero;
        }
    }
}