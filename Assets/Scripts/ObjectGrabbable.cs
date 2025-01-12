using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class ObjectGrabbable : MonoBehaviour
{
    [FormerlySerializedAs("lerpSpeed")] [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private bool affectedByGravity = true;
    private Rigidbody _rb;
    private Transform _grabPointTransform;

    public bool Grabbable { get; private set; }

    private void Awake()
    {
        Grabbable = true;
        _rb = GetComponent<Rigidbody>();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        _grabPointTransform = objectGrabPointTransform;
        Grabbable = false;
        _rb.useGravity = false;
    }

    public void Drop()
    {
        _grabPointTransform = null;
        Grabbable = true;
        _rb.useGravity = affectedByGravity; // For object that may not be affected by gravity in puzzles in the future
    }


    private void FixedUpdate()
    {
        if (_grabPointTransform)
        {

            Vector3 force = new Vector3(
                _grabPointTransform.position.x - _rb.position.x, 
                _grabPointTransform.position.y - _rb.position.y,
                _grabPointTransform.position.z - _rb.position.z);
            _rb.linearVelocity = force * moveSpeed;
        }
    }
}
