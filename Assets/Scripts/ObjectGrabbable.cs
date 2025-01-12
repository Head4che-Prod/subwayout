using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class ObjectGrabbable : MonoBehaviour
{
    [FormerlySerializedAs("lerpSpeed")] [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private bool affectedByGravity = true;
    protected Rigidbody Rb;
    private Transform _grabPointTransform;

    public bool Grabbable { get; private set; }

    private void Awake()
    {
        Grabbable = true;
        Rb = GetComponent<Rigidbody>();
        Rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        if (_grabPointTransform)
        {
            Vector3 force = new Vector3(
                _grabPointTransform.position.x - Rb.position.x, 
                _grabPointTransform.position.y - Rb.position.y,
                _grabPointTransform.position.z - Rb.position.z);
            Rb.linearVelocity = force * moveSpeed;
        }
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        _grabPointTransform = objectGrabPointTransform;
        Grabbable = false;
        Rb.useGravity = false;

        if (!gameObject.TryGetComponent(out Outline _))
        {
            var outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 5f;
        }
    }

    public virtual void Drop()
    {
        _grabPointTransform = null;
        Grabbable = true;
        Rb.useGravity = affectedByGravity; // For object that may not be affected by gravity in puzzles in the future
    }

}
