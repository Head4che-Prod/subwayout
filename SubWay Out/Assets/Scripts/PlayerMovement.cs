using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : NetworkBehaviour
{
	[Header("Links to Unity Objects")]
	[SerializeField] private Transform orientation;
	[SerializeField] private LayerMask whatIsGround;  // Consider changing to tag instead
	
	[Header("Speed constants (debug)")]
	[SerializeField] private float groundDrag = 5f;
	[SerializeField] private float airMultiplier = 1.25f;
	[SerializeField] private float walkSpeed = 50f;
	[SerializeField] private float sprintSpeed = 70f;
	
	[Header("Env constants (debug)")]
	[SerializeField] private float playerHeight = 10f;
	[SerializeField] private float maxSlopeAngle = 40f;
	
	[Header("Movement")]
	private float _moveSpeed;
	private Vector3 _moveDirection;
	private MovementState _movementState;
	public enum MovementState
	{
		Walking,
		Sprinting,
		Air
	}
	private bool IsOnGround => _movementState == MovementState.Walking || _movementState == MovementState.Sprinting;

	[Header("Slope Handling")] 
	private RaycastHit _slopeHit;
	private Vector3 CurrentSlopeDirection => Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
	private bool IsOnSlope
	{
		get {
			if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, playerHeight * 0.5f + 6.5f))
			{
				float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
				return angle < maxSlopeAngle && angle != 0;
			}

			return false;
		}
	}
	
	[Header("Miscellaneous")] 
	private Rigidbody _rb;
	private InputAction _moveAction;
	private InputAction _sprintAction;
	
    void Start()
    {
	    _rb = GetComponent<Rigidbody>();
	    _rb.freezeRotation = true;
	    
	    _moveAction = InputSystem.actions.FindAction("Player/Move");
	    _sprintAction = InputSystem.actions.FindAction("Player/Sprint");
	    
	    _movementState = MovementState.Walking;
    }

    void FixedUpdate()
    {
	    if (!IsLocalPlayer) return;
	    
	    _moveAction = InputSystem.actions.FindAction("Player/Move");
	    _sprintAction = InputSystem.actions.FindAction("Player/Sprint");
	    
	    SpeedCtrl();
	    StateHandler();

	    _rb.linearDamping = IsOnGround ? groundDrag : 0;
	    MovePlayer();
    }


    private void MovePlayer()
    {
	    Vector2 inputDirection = _moveAction.ReadValue<Vector2>();
	    _moveDirection = orientation.forward * inputDirection.y + orientation.right * inputDirection.x;
	    
	    // Slope
	    if (IsOnSlope)
	    {
		    _rb.AddForce(CurrentSlopeDirection * (_moveSpeed * 20f), ForceMode.Force);
		    if (_rb.linearVelocity.y > 0)
			    _rb.AddForce(Vector3.down * 100f, ForceMode.Force);
	    }
	    
	    // Ground
	    if (IsOnGround)
			_rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
	    // Air
	    else if (!IsOnGround)
	    {
		    _rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f * airMultiplier), ForceMode.Force);
	    }
	    
	    _rb.useGravity = !IsOnSlope;
    }

    private void SpeedCtrl()
    {
	    // Speed limit on slope
	    if (IsOnSlope)
	    {
		    if (_rb.linearVelocity.magnitude > _moveSpeed)
			    _rb.linearVelocity = _rb.linearVelocity.normalized * _moveSpeed;
	    }
	    // Speed limit on ground or in air
	    else
	    {
		    Vector3 flatVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

		    if (flatVelocity.magnitude > _moveSpeed)
		    {
			    Vector3 limitedVelocity = flatVelocity.normalized * _moveSpeed;
			    _rb.linearVelocity = new Vector3(limitedVelocity.x, _rb.linearVelocity.y, limitedVelocity.z);
		    }
	    }
    }



    private void StateHandler()
    {
	    bool onGround = Physics.Raycast(transform.position, -Vector3.up, playerHeight * 0.5f + 6.5f, whatIsGround);
	    
	    // Sprint
	    if (onGround && _sprintAction.IsPressed())
	    {
		    _movementState = MovementState.Sprinting;
		    _moveSpeed = sprintSpeed;
	    }
	    // Walk
	    else if (onGround)
	    {
		    _movementState = MovementState.Walking;
		    _moveSpeed = walkSpeed;
	    }
	    // Air
	    else
	    {
		    _movementState = MovementState.Air;
	    }
    }
}
