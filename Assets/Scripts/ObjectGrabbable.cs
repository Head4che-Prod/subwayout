using JetBrains.Annotations;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkRigidbody))]
[RequireComponent(typeof(NetworkObject))]
public class ObjectGrabbable : NetworkBehaviour
{
    [FormerlySerializedAs("lerpSpeed")] [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private bool affectedByGravity = true;
    protected Rigidbody Rb { get; private set; }
    private Transform GrabPointTransform { get; set; }
    [CanBeNull] protected Transform HolderCameraTransform { get; private set; }
    
    private Vector3 GrabPointPosition => GrabPointTransform.position;
    protected bool IsGrabbable;

    public virtual bool Grabbable   // This can be overridden
    {
        get => IsGrabbable;
        private set => IsGrabbable = value;
    }

    public void Start()
    {
        Grabbable = true;
        
        // Warning: All rigidbody settings in this section must be copied / adapted for HanoiGrabbable
        Rb = GetComponent<NetworkRigidbody>().Rigidbody;
        Rb.interpolation = RigidbodyInterpolation.Extrapolate;
    }

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
            Rb.linearVelocity = force * moveSpeed;
            // Rb.MovePosition(_grabPointTransform.position);
        }
    }

    public virtual void Grab(Transform objectGrabPointTransform, Transform playerCamera)
    {
        // Debug.Log($"Owner {OwnerClientId} attempted grabbing {name}");
        GrabPointTransform = objectGrabPointTransform;
        Grabbable = false;
        Rb.useGravity = false;
        HolderCameraTransform = playerCamera;
    }

    public virtual void Drop()
    {
        GrabPointTransform = null;
        Grabbable = true;
        Rb.useGravity = affectedByGravity; // For object that may not be affected by gravity in puzzles in the future
        HolderCameraTransform = null;
    }
}
