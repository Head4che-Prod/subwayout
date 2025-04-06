using System;
using Objects;
using Prefabs.Player;
using Prefabs.Puzzles.Hanoi.Debugs;
using UnityEngine;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiGrabbable : ObjectGrabbable
    {
        public override bool Grabbable => !HanoiTowers.Instance.InUse.Value && IsGrabbable.Value;  // Checks whether the hanoi towers are already in use.
        
        public override void Grab(PlayerObject player)
        {
            base.Grab(player);
            //Debug.Log(HanoiCollider.DebugGrid());
            HanoiTowers.Instance.InUse.Value = true;
        }
        public override void Drop()
        {
            base.Drop();
            
            // Debug.Log($"Let go of {name} at ({Rb.position.x}, {Rb.position.y}, {Rb.position.z})");
            HanoiHitbox.ResetBall(transform);
            HanoiTowers.Instance.InUse.Value = false;
        }

        public override Vector3 CalculateMovementForce()
        {
            if (Physics.Raycast(Owner.playerCamera.transform.position, Owner.playerCamera.transform.forward,
                    out RaycastHit hit, 12f, 1 << 3))  // Only let layer 3 pass
            {
                // Put vector in local space (to access data relative to the gamed plane)
                Vector3 direction = HanoiTowers.Instance.transform.InverseTransformVector(hit.point - transform.position);
                
                if (HanoiTowers.Instance.IsInDebugMode)
                {
                    TargetPosition.Instance.SetPosition(0, hit.point);
                    TargetPosition.Instance.SetPosition(1, transform.position);
                }
                direction.y = 0;

                
                if (direction.magnitude > 0.01f) // Only move if movement will actually make a difference
                {
                    ushort movementMode = 0;
                    //Vector3 hitPointLocal = HanoiTowers.Instance.transform.InverseTransformDirection(hit.point);
                    //Vector3 positionLocal =
                    //    HanoiTowers.Instance.transform.InverseTransformDirection(transform.position);

                    if (direction.x / direction.z > 2f)
                    {
                        movementMode = 1;
                        direction.z = 0;
                    }
                    else if (direction.z / direction.x > 2f)
                    {
                        movementMode = 2;
                        direction.x = 0;
                    }
                    
                    // Put vector back into global space
                    direction = HanoiTowers.Instance.transform.TransformVector(direction.normalized * 0.25f);
                
                    if (HanoiTowers.Instance.IsInDebugMode)
                    {
                        MovementVector.Instance.startColor = movementMode switch
                        {
                            0 => Color.magenta,
                            1 => Color.red,
                            2 => Color.blue,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        MovementVector.Instance.SetPosition(0, transform.position);
                        MovementVector.Instance.SetPosition(1, transform.position + direction);
                    }
                
                    return direction;
                }
                else
                {
                    if (HanoiTowers.Instance.IsInDebugMode)
                    {
                        MovementVector.Instance.SetPosition(0, transform.position);
                        MovementVector.Instance.SetPosition(1, transform.position);
                    }

                    return Vector3.zero;
                }
            }

            Drop();
            return Vector3.zero;
        }
    }
}