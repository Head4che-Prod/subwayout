using Prefabs.GameManagers;
using Prefabs.Player;
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
    [RequireComponent(typeof(ObjectOutline))]
    public class ObjectGrabbable : ObjectInteractable, IResettablePosition
    {
        [Header("Physics")] [FormerlySerializedAs("lerpSpeed")] [SerializeField]
        private float moveSpeed = 2.0f;
        
        /// <summary>
        /// How strict the collision detection is for the grabbed object. Use lower for objects that need to be moved precisely, and higher for objects that can be moved fast.
        /// </summary>
        protected virtual CollisionDetectionMode CollisionDetectionMode => CollisionDetectionMode.ContinuousDynamic;

        [SerializeField] private bool affectedByGravity = true;
        
        [Header("Visuals")]
        [SerializeField] private bool canBeHighlighted = true;
        
        private Rigidbody Rb { get; set; }
        
        public PlayerObject Owner { get; private set; }

        private Vector3 GrabPointPosition => Owner.grabPointTransform.position;
        protected NetworkVariable<bool> IsGrabbable;
        
        public virtual bool Grabbable // This can be overridden
        {
            get => IsGrabbable.Value;
            private set => IsGrabbable.Value = value;
        }
        
        protected ObjectOutline Outline;
        

        public void Awake()
        {
            IsGrabbable = new NetworkVariable<bool>(true);
        }
        
        public void Start()
        {
            ((IResettablePosition)this).RegisterInitialState(transform.position, transform.rotation);
            SetGrabbableServerRpc(true);
            // Warning: All rigidbody settings in this section must be copied / adapted for HanoiGrabbable
            Rb = GetComponent<NetworkRigidbody>().Rigidbody;
            Rb.interpolation = RigidbodyInterpolation.Extrapolate;
            Rb.collisionDetectionMode = CollisionDetectionMode;
            
            Outline = GetComponent<ObjectOutline>();
            Outline.enabled = false;

            Rb.isKinematic = !IsHost;
            IsGrabbable.OnValueChanged += HandleGravity;
        }
        
        /// <summary>
        /// This method ask the server to set grabbability of an object over the Network.
        /// </summary>
        /// <param name="value">Boolean of Grabbable.</param>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void SetGrabbableServerRpc(bool value) => Grabbable = value;

        /// <summary>
        /// Sets gravity on all clients when an object is grabbed, used when <see cref="IsGrabbable"/>'s value is changed.
        /// </summary>
        /// <param name="_">Old value of IsGrabbable</param>
        /// <param name="isGrabbable">New value of IsGrabbable</param>
        private void HandleGravity(bool _, bool isGrabbable) => Rb.useGravity = isGrabbable && affectedByGravity;
        
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
            if (Owner)
            {
                Vector3 force = CalculateMovementForce();
                MoveGrabbedObjectServerRpc(force * moveSpeed); // todo: Speed need to be modified!
            }
        }

        /// <summary>
        /// This method ask the server to apply a velocity to a grabbed object.
        /// </summary>
        /// <param name="move">Vector3 velocity of grabbed object.</param>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void MoveGrabbedObjectServerRpc(Vector3 move)
        {
            Rb.linearVelocity = move;
        }

        /// <summary>
        /// Make players grab the targeted object.
        /// </summary>
        /// <param name="player"><see cref="PlayerObject"/> holding the item".</param>
        public virtual void Grab(PlayerObject player)
        {
            Owner = player;
            if (Owner.grabPointTransform is null)
            {
                Debug.LogError("objectGrabPointTransform is null");
                return;
            }

            if (Owner.playerCamera is null)
            {
                Debug.LogError("playerCamera is null");
                return;
            }

            // Debug.Log($"Owner {OwnerClientId} attempted grabbing {name}");
            SetGrabbableServerRpc(false);
            
            if (canBeHighlighted)
            {
                ObjectHighlightManager.RegisterHighlightableObject(Outline);
                Outline.enabled = ObjectHighlightManager.HighlightEnabled;
            }
        }

        /// <summary>
        /// Make players drop the grabbed object.
        /// </summary>
        public virtual void Drop()
        {
            Owner.Interaction.GrabbedObject = null;
            Owner = null;
            SetGrabbableServerRpc(true);
        }
        
        public override void OnDestroy()
        {
            base.OnDestroy();
            IsGrabbable.OnValueChanged -= HandleGravity;
        }
        
        // Position reset interface
        public Vector3 InitialPosition { get; set; }
        public Quaternion InitialRotation { get; set; }

        public virtual void ResetPosition()
        {
            if (Owner != null)
                Drop();
            
            transform.position = InitialPosition;
            transform.rotation = InitialRotation;
        }

    }
}