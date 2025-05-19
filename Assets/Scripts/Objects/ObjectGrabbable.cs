using JetBrains.Annotations;
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
    public class ObjectGrabbable : OffstageNetworkBehaviour, IResettablePosition, IObjectGrabbable
    {
        [Header("Physics")] [FormerlySerializedAs("lerpSpeed")]
        public float moveSpeed = 2.0f;
        
        /// <summary>
        /// How strict the collision detection is for the grabbed object. Use lower for objects that need to be moved precisely, and higher for objects that can be moved fast.
        /// </summary>
        protected virtual CollisionDetectionMode CollisionDetectionMode => CollisionDetectionMode.ContinuousDynamic;

        [SerializeField] private bool affectedByGravity = true;
        
        [Header("Visuals")]
        [SerializeField] protected bool canBeHighlighted = true;
        
        public Rigidbody Rb { get; private set; }

        protected NetworkVariable<bool> IsGrabbable = new NetworkVariable<bool>(true);
        
        [SerializeField] [CanBeNull] string soundEffectName = null;

        
        public virtual bool Grabbable // This can be overridden
        {
            get => IsGrabbable.Value;
            set => IsGrabbable.Value = value;
        }

        public ObjectGrabbable GrabbedObject => this;

        private ObjectOutline _outline;
        

        public override void Awake()
        {
            base.Awake();
            IsGrabbable = new NetworkVariable<bool>(true);
        }
        
        public virtual void Start()
        {
            InitialPosition = transform.position;
            InitialRotation = transform.rotation;
            ObjectPositionManager.Singleton.ResettableObjects.Add(this);
            // Warning: All rigidbody settings in this section must be copied / adapted for HanoiGrabbable
            Rb = GetComponent<NetworkRigidbody>().Rigidbody;
            Rb.interpolation = RigidbodyInterpolation.Extrapolate;
            Rb.collisionDetectionMode = CollisionDetectionMode;
            
            _outline = GetComponent<ObjectOutline>();
            _outline.enabled = false;

            Rb.isKinematic = !IsHost;
            IsGrabbable.OnValueChanged += HandleGravity;
        }

        /// <summary>
        /// Sets gravity on all clients when an object is grabbed, used when <see cref="IsGrabbable"/>'s value is changed.
        /// </summary>
        /// <param name="_">Old value of IsGrabbable</param>
        /// <param name="isGrabbable">New value of IsGrabbable</param>
        private void HandleGravity(bool _, bool isGrabbable) => Rb.useGravity = isGrabbable && affectedByGravity;
        
        /// <returns><see cref="Vector3"/> of the difference between player's <see cref="GrabPointPosition"/> and the current grabbed object positions.</returns>
        public virtual Vector3 CalculateMovementForce(PlayerObject playerGrabbing)
        {
            return new Vector3(
                playerGrabbing.grabPointTransform.position.x - transform.position.x,
                playerGrabbing.grabPointTransform.position.y - transform.position.y,
                playerGrabbing.grabPointTransform.position.z - transform.position.z);
        }
        
        public virtual void Grab()
        {
            // Debug.Log($"Owner {OwnerClientId} attempted grabbing {name}");
            GrabServerRpc(NetworkManager.Singleton.LocalClientId, NetworkObjectId);
            
            if(soundEffectName is not null and not "")
                SoundManager.Singleton.PlaySoundRpc(soundEffectName, transform.position, 1f); // Play a sound on grab
            
            if (canBeHighlighted)
            {
                ObjectHighlightManager.RegisterHighlightableObject(NetworkObjectId);
                EnableHighlightRpc(ObjectHighlightManager.HighlightEnabled);
            }
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void EnableHighlightRpc(bool isActivated)
        {
            _outline.enabled = isActivated;
        }
        
        /// <summary>
        /// Asks the host to register an object as grabbed.
        /// </summary>
        /// <param name="clientId">Player that grabbed the object.</param>
        /// <param name="objectId">Network id of the object grabbed.</param>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void GrabServerRpc(ulong clientId, ulong objectId)
        {
            GrabbedObjectManager.PlayerGrab(clientId, objectId);
            Grabbable = false;
        }

        public virtual void Drop()
        {
            PlayerInteract.LocalPlayerInteract.GrabbedObject = null;
            DropServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        /// <summary>
        /// Asks the host to register an object as dropped.
        /// </summary>
        /// <param name="clientId">Player that grabbed the object.</param>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void DropServerRpc(ulong clientId) => DropServerLogic(clientId);

        /// <summary>
        /// Handles the logic used by the host to register an object as dropped.
        /// </summary>
        /// <param name="clientId">Player that grabbed the object.</param>
        protected virtual void DropServerLogic(ulong clientId)
        {
            GrabbedObjectManager.PlayerDrop(clientId);
            Grabbable = true;
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
            Drop();
            
            transform.position = InitialPosition;
            transform.rotation = InitialRotation;
        }

    }
}