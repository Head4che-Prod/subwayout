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
    protected Rigidbody Rb;
    private Transform _grabPointTransform;

    protected bool IsGrabbable;

    public virtual bool Grabbable   // This can be overridden
    {
        get => IsGrabbable;
        private set => IsGrabbable = value;
    }

    public void Start()
    {
        Grabbable = true;
        Rb = GetComponent<NetworkRigidbody>().Rigidbody;
        Rb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        if (_grabPointTransform)
        {
            Rb.MovePosition(_grabPointTransform.position);
        }
    }

    public virtual void Grab(Transform objectGrabPointTransform)
    {
        
        Debug.Log($"Owner {OwnerClientId} attempted grabbing {name}");
        _grabPointTransform = objectGrabPointTransform;
        Grabbable = false;
        Rb.useGravity = false;
    }

    public virtual void Drop()
    {
        _grabPointTransform = null;
        Grabbable = true;
       Rb.useGravity = affectedByGravity; // For object that may not be affected by gravity in puzzles in the future
    }

}
