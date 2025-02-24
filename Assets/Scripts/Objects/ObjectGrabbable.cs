using JetBrains.Annotations;
using Objects.Actionables;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

namespace Objects
{
    /// <summary>
    /// Allow players to grab objects.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NetworkRigidbody))]
    [RequireComponent(typeof(NetworkObject))]
    public class ObjectGrabbable : ObjectInteractive
    {
        [Header("Physics")]
        [FormerlySerializedAs("lerpSpeed")] [SerializeField] private float moveSpeed = 2.0f;
        [SerializeField] private bool affectedByGravity = true;
        protected Rigidbody Rb { get; private set; }
        private Transform GrabPointTransform { get; set; }
        [CanBeNull] protected Transform HolderCameraTransform { get; private set; }
    
        private Vector3 GrabPointPosition => GrabPointTransform.position;
        protected NetworkVariable<bool> IsGrabbable = new (true);
        
        // Used for potential convert to actionable.
        [Header("Actionable")] public ActionableType? ConvertActionableType;

        public virtual bool Grabbable   // This can be overridden
        {
            get => IsGrabbable.Value;
            private set => IsGrabbable.Value = value;
        }

        /// <summary>
        /// This method ask the server to set grabbability of an object over the Network.
        /// </summary>
        /// <param name="value">Boolean of Grabbable.</param>
        [ServerRpc(RequireOwnership = false)]
        private void SetGrabbableServerRpc(bool value) => Grabbable = value;

        public void Start()
        {
            SetGrabbableServerRpc(true);
            // Warning: All rigidbody settings in this section must be copied / adapted for HanoiGrabbable
            Rb = GetComponent<NetworkRigidbody>().Rigidbody;
            Rb.interpolation = RigidbodyInterpolation.Extrapolate;
        }
        
        /// <returns><see cref="Vector3"/> of the difference between player's <see cref="GrabPointPosition"/> and the current grabbed object positions.</returns>
        public virtual Vector3 CalculateMovementForce()
        {
            return new Vector3(
                GrabPointPosition.x - transform.position.x, 
                GrabPointPosition.y - transform.position.y,
                GrabPointPosition.z - transform.position.z);
        }
    
        private void FixedUpdate()
        {
            if (GrabPointTransform)
            {
                Vector3 force = CalculateMovementForce();
                MoveGrabbedObjectServerRpc(force * moveSpeed); // todo: Speed need to be modified!
                // Rb.linearVelocity = force * moveSpeed;
                // Rb.MovePosition(_grabPointTransform.position);
            }
        }

        /// <summary>
        /// This method ask the server to apply a velocity to a grabbed object.
        /// </summary>
        /// <param name="move">Vector3 velocity of grabbed object.</param>
        [ServerRpc(RequireOwnership = false)]
        private void MoveGrabbedObjectServerRpc(Vector3 move)
        {
            Rb.linearVelocity = move;
        }

        /// <summary>
        /// Make players grab the targeted object.
        /// </summary>
        /// <param name="objectGrabPointTransform"><see cref="Transform"/> of the player's "grab point".</param>
        /// <param name="playerCamera"><see cref="Transform"/> of the player's camera.</param>
        public virtual void Grab(Transform objectGrabPointTransform, Transform playerCamera)
        {
            // Debug.Log($"Owner {OwnerClientId} attempted grabbing {name}");
            GrabPointTransform = objectGrabPointTransform;
            SetGrabbableServerRpc(false);
            Rb.useGravity = false;
            HolderCameraTransform = playerCamera;
        }

        /// <summary>
        /// Make players drop the grabbed object.
        /// </summary>
        public virtual void Drop()
        {
            GrabPointTransform = null;
            SetGrabbableServerRpc(true);
            Rb.useGravity = affectedByGravity; // For object that may not be affected by gravity in puzzles in the future
            HolderCameraTransform = null;
        }

        /// <summary>
        /// Convert to <see cref="ObjectActionable"/> and destroy the current component.
        /// </summary>
        /// <param name="placeholder"><see cref="ObjectPlaceholder"/> of the new location of the ObjectActionable</param>
        /// <returns><see cref="ObjectActionable"/></returns>
        [CanBeNull]
        public ObjectActionable ToActionable(ObjectPlaceholder placeholder)
        {
            return placeholder.Free ? ToActionableServerRpc(placeholder.transform) : null;
        }

        /// <summary>
        /// Convert to ObjectActionable and destroy the current component by the server.
        /// </summary>
        /// <param name="newLocation"><see cref="Transform"/> of the new location of the ObjectActionable</param>
        /// <param name="type"></param>
        /// <returns><see cref="ObjectActionable"/></returns>
        [ServerRpc(RequireOwnership = false)]
        [CanBeNull]
        private ObjectActionable ToActionableServerRpc(Transform newLocation)
        {
            ObjectActionable newActionable = null;
            switch (ConvertActionableType)
            {
                case ActionableType.EmergencyTrigger:
                    newActionable = gameObject.AddComponent<ActionableEmergencyTrigger>();
                    break;
                case ActionableType.Backpack:
                    newActionable = gameObject.AddComponent<ActionableEmergencyTrigger>();
                    break;
                case ActionableType.MetroDoors:
                    newActionable = gameObject.AddComponent<ActionableEmergencyTrigger>();
                    break;
                case ActionableType.Trapdoor:
                    newActionable = gameObject.AddComponent<ActionableEmergencyTrigger>();
                    break;
                case ActionableType.AdvertisingDisplay:
                    newActionable = gameObject.AddComponent<ActionableEmergencyTrigger>();
                    break;
            }

            if (newActionable is null)
                return null;
            
            // Edit properties of newActionable;
            newActionable.transform.position = newLocation.position;
            newActionable.transform.rotation = newLocation.rotation;
            newActionable.transform.localScale  = newLocation.localScale;
            
            Destroy(gameObject.GetComponent<ObjectGrabbable>());
            return newActionable;
        }
        
    }
}
