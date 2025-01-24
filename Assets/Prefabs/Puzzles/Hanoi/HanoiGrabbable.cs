using System;
using Prefabs.Puzzles.Hanoi.Debugs;
using UnityEngine;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiGrabbable : ObjectGrabbable
    {
        private static bool _useable = true;
        //public override bool Grabbable => _useable && IsGrabbable;  // Checks whether the hanoi towers are already in use.

        protected override Vector3 GrabPointPosition
        {
            get
            {
                if (HolderCameraTransform == null)
                    throw new NullReferenceException("Could not find camera holding the hanoi element.");
                
                // Computes the vector normal to the game puzzle plane.
                Vector3 normal = (HanoiTowers.ActiveTowersGameObject.transform.rotation * Quaternion.Euler(90, 0, 0)) * Vector3.up;  
                float a = normal.x;
                float b = normal.y;
                float c = normal.z;
                Debug.Log($"{a}x + {b}y + {c}z");

                
                // Point in the plane
                float xT = HanoiTowers.ActiveTowersGameObject.transform.position.x;
                float yT = HanoiTowers.ActiveTowersGameObject.transform.position.y;
                float zT = HanoiTowers.ActiveTowersGameObject.transform.position.z;
                
                // Points forming the raycast
                float xA = HolderCameraTransform.position.x;
                float yA = HolderCameraTransform.position.y;
                float zA = HolderCameraTransform.position.z;
                
                float xB = GrabPointTransform.position.x;
                float yB = GrabPointTransform.position.y;
                float zB = GrabPointTransform.position.z;
                
                float d = -(a * xT + b * yT + c * zT);
                
                // Don't ask me how the debug works. Trust me, it does NOT NO IT DOESNT THATS WHY IT DIDNT WORK.
                DebugPlane.Instance.transform.rotation = Quaternion.AngleAxis(
                    Mathf.Acos(Vector3.Dot(Vector3.up, normal.normalized)) * Mathf.Rad2Deg,
                    Vector3.Cross(Vector3.up, normal.normalized)
                );
                DebugPlane.Instance.transform.position = new Vector3(0, 0, -d/c);
                GrabPointVisualizer.Instance.transform.position = GrabPointTransform.position;
                MoveLine.Instance.SetPosition(0, new Vector3(xA, yA, zA));
                MoveLine.Instance.SetPosition(1, new Vector3(xB, yB, zB));
                
                // Rotating 90 degrees along the x axis to account for Hanoi's base shape
                //(yA, yB, zA, zB) = (-zA, -zB, yA, yB);
                
                // Compute intersection between ray and plane
                
                float denominator = a * (xB - xA) + b * (yB - yA) + c * (zB - zA);
                
                if (denominator == 0f)
                    return new Vector3(xT, yT, zT);
                
                float numerator = a * xA + b * yA + c * zA + d;
                float t = numerator / denominator;

                Vector3 resultVectorRotated = new Vector3(
                    xA + t * (xB - xA),
                    yA + t * (yB - yA),
                    zA + t * (zB - zA)
                );

                return resultVectorRotated;
            }
        }
        
        public override void Grab(Transform objectGrabPointTransform, Transform playerCamera)
        {
            base.Grab(objectGrabPointTransform, playerCamera);
            Debug.Log(HanoiCollider.DebugGrid());
            _useable = false;
        }
        public override void Drop()
        {
            base.Drop();
            
            // Debug.Log($"Let go of {name} at ({Rb.position.x}, {Rb.position.y}, {Rb.position.z})");
            HanoiCollider.ResetBall(transform);
            _useable = true;
        }
    }
}