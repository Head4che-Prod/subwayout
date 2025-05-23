using System;
using System.Collections;
using Objects;
using Prefabs.Player;
using Prefabs.Puzzles.Hanoi.Debugs;
using UnityEngine;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiPiece : ObjectGrabbable
    {
        protected override CollisionDetectionMode CollisionDetectionMode => CollisionDetectionMode.Discrete;

        public override bool Grabbable =>
            !HanoiTowers.Instance.GetUsageState() && IsGrabbable.Value
            && CheckNoPieceOnTop();

        /// <summary>
        /// Weight of the ball. Balls can not be placed on heavier ones.
        /// </summary>
        [Header("Hanoi")] [SerializeField, Range(0, 2)]
        public int weight;

        /// <summary>
        /// Checks whether the piece is the highest on its bar.
        /// </summary>
        /// <returns>Whether there is no piece above.</returns>
        private bool CheckNoPieceOnTop()
        {
            for (int bar = 0; bar < 3; bar++)
            for (int level = 2; level  >= 0; level--)
            {
                HanoiPiece ball = HanoiTowers.Instance.ColliderGrid[bar, level].containedBall;
                if (ball)
                {
                    if (ball == this)
                        return true;
                    break;
                }
            }

            return false;
        }

        public override void Start()
        {
            base.Start();
            gameObject.transform.localPosition = new Vector3(0.12f, 0.0135f, 0.033f + 0.03f * weight);
            StartCoroutine(RegisterBall());
        }

        /// <summary>
        /// Waits until the collider grid is successfully initialized before registering the ball.
        /// </summary>
        private IEnumerator RegisterBall()
        {
            yield return new WaitUntil(() => HanoiTowers.Instance.ColliderGrid[0, 2 - weight] is not null);
            HanoiTowers.Instance.ColliderGrid[0, 2 - weight].containedBall = this;
        }

        public override void Grab()
        {
            HanoiTowers.Instance.SetUsageState(true);
            base.Grab();
        }

        public override void Drop()
        {
            HanoiTowers.Instance.SetUsageState(false);
            base.Drop();
        }

        protected override void DropServerLogic(ulong clientId)
        {
            base.DropServerLogic(clientId);

            StartCoroutine(ResetPositionCoroutine());
        }

        private IEnumerator ResetPositionCoroutine()
        {
            Vector3 position = transform.localPosition;
            foreach (HanoiHitbox hitbox in HanoiTowers.Instance.ColliderGrid)
                if (hitbox.containedBall?.weight == weight)
                {
                    position = new Vector3(
                        hitbox.gameObject.transform.localPosition.x,
                        0.0135f,
                        hitbox.gameObject.transform.localPosition.z
                    );
                    break;
                }

            // transform.localPosition = position;
            yield return new WaitForFixedUpdate();
            transform.localPosition = position;
        }

        public override Vector3 CalculateMovementForce(PlayerObject playerGrabbing)
        {
            if (IsGrabbable.Value)
                return Vector3.zero;

            if (Physics.Raycast(playerGrabbing.playerCamera.transform.position,
                    playerGrabbing.playerCamera.transform.forward,
                    out RaycastHit hit, 12f, 1 << 3)) // Only let layer 3 pass
            {
                // Put vector in local space (to access data relative to the gamed plane)
                Vector3 direction =
                    HanoiTowers.Instance.transform.InverseTransformVector(hit.point - transform.position);

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
                    direction = HanoiTowers.Instance.transform.TransformVector(direction.normalized) * 0.25f;

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

        public override void ResetPosition()
        {
            HanoiTowers.Instance.ResetPositions();
            base.ResetPosition();
        }
    }
}