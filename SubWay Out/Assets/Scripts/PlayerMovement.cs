using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Movement")]
	private float moveSpeed;
	public float walkSpeed;
	public float sprintSpeed;

	public float groundDrag;

	public float airMultiplier;
	
	public Transform orientation;
	
	public MovementState state;

	[Header("Keybindings")] 
	public KeyCode sprintKey = KeyCode.LeftShift;
	
	[Header("Ground Check")] 
	public float playerHeight;
	public LayerMask whatIsGround;
	private bool grounded;

	[Header("Slope Handling")] 
	public float maxSlopeAngle;
	private RaycastHit slopeHit;

	
	float horizontalInput;
	float verticalInput;

	private Vector3 moveDirection;
	
	Rigidbody rb;
	
    void Start()
    {
	    rb = GetComponent<Rigidbody>();
	    rb.freezeRotation = true;
    }
    
    void Update()
    {
	    grounded = Physics.Raycast(transform.position, -Vector3.up, playerHeight * 0.5f + 6.5f, whatIsGround);
	    
	    KeyboardInput();
	    SpeedCtrl();
	    StateHandler();
	    
	    if (grounded)
		    rb.linearDamping = groundDrag;
	    else
		    rb.linearDamping = 0;
    }

    void FixedUpdate()
    {
	    MovePlayer();
    }

    private void KeyboardInput()
    {
	    horizontalInput = Input.GetAxisRaw("Horizontal");
	    verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
	    moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
	    // Slope
	    if (OnSlope())
	    {
		    rb.AddForce(GetSlopeMoveDirection() * (moveSpeed * 20f), ForceMode.Force);
		    if (rb.linearVelocity.y > 0)
			    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
	    }
	    
	    // Ground
	    if (grounded)
			rb.AddForce(moveDirection.normalized * (moveSpeed * 10f), ForceMode.Force);
	    // Air
	    else if (!grounded)
		    rb.AddForce(moveDirection.normalized * (moveSpeed * 10f * airMultiplier), ForceMode.Force);

	    rb.useGravity = !OnSlope();
    }

    private void SpeedCtrl()
    {
	    // Speed limit on slope
	    if (OnSlope())
	    {
		    if (rb.linearVelocity.magnitude > moveSpeed)
			    rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
	    }
	    // Speed limit on ground or in air
	    else
	    {
		    Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

		    if (flatVelocity.magnitude > moveSpeed)
		    {
			    Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
			    rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
		    }
	    }
    }

    private bool OnSlope()
    {
	    if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 6.5f))
	    {
		    float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
		    return angle < maxSlopeAngle && angle != 0;
	    }

	    return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
	    return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void StateHandler()
    {
	    // Sprint
	    if (grounded && Input.GetKey(sprintKey))
	    {
		    state = MovementState.Sprinting;
		    moveSpeed = sprintSpeed;
	    }
	    // Walk
	    else if (grounded)
	    {
		    state = MovementState.Walking;
		    moveSpeed = walkSpeed;
	    }
	    // Air
	    else
	    {
		    state = MovementState.Air;
	    }
    }
    
    public enum MovementState
    {
	    Walking,
	    Sprinting,
	    Air
    }
}
