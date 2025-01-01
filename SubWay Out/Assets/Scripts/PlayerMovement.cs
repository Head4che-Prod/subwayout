using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Movement")]
	private float moveSpeed;
	public float walkSpeed;
	public float sprintSpeed;

	public float groundDrag;

	public float airMultiplier;

	[Header("Keybindings")] 
	public KeyCode sprintKey = KeyCode.LeftShift;
	
	[Header("Ground Check")] 
	public float playerHeight;
	public LayerMask whatIsGround;
	private bool grounded;

	public Transform orientation;

	float horizontalInput;
	float verticalInput;

	private Vector3 moveDirection;
	
	Rigidbody rb;
	public MovementState state;
	
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
	    if (grounded)
			rb.AddForce(moveDirection.normalized * (moveSpeed * 10f), ForceMode.Force);
	    else if (!grounded)
		    rb.AddForce(moveDirection.normalized * (moveSpeed * 10f * airMultiplier), ForceMode.Force);
	    
    }

    private void SpeedCtrl()
    {
	    Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

	    if (flatVelocity.magnitude > moveSpeed)
	    {
		    Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
		    rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
	    }
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
