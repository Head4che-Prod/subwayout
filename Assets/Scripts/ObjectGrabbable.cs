using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 10f;
    private Rigidbody _rb;
    private Transform _grabPointTransform;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        _grabPointTransform = objectGrabPointTransform;
        _rb.useGravity = false;
    }

    public void Drop()
    {
        _grabPointTransform = null;
        _rb.useGravity = true;
    }

    public bool Grabbable()
    {
        return _rb.useGravity;
    }

    private void FixedUpdate()
    {
        if (_grabPointTransform)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, _grabPointTransform.position,
                Time.deltaTime * lerpSpeed);
            _rb.MovePosition(newPosition);
        }
    }
}
