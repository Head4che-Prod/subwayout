using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[Header("Movement")]
		private float _moveSpeed;
		public float walkSpeed;
		public float sprintSpeed;
		private Vector3 _moveDirection;
		
	
		public float groundDrag;
		public float airMultiplier;
		public Transform orientation;

		[Header("Keybindings")] 
		private InputAction _movementInput;
		private InputAction _sprintInput;
	
		[Header("Ground Check")] 
		public float playerHeight;
		public LayerMask whatIsGround;
		private bool _grounded;

		private float _horizontalInput;
		private float _verticalInput;

		private Rigidbody _rb;
	
		void Start()
		{
			_movementInput = InputSystem.actions.FindAction("Player/Move");
			_sprintInput = InputSystem.actions.FindAction("Player/Sprint");
			_rb = GetComponent<Rigidbody>();
			_rb.freezeRotation = true;
		}
    
		void Update()
		{
			_grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
	    
			KeyboardInput();
			SpeedCtrl();
			StateHandler();
	    
			if (_grounded)
				_rb.linearDamping = groundDrag;
			else
				_rb.linearDamping = 0;
		}

		void FixedUpdate()
		{
			MovePlayer();
		}

		private void KeyboardInput()
		{
			Vector2 moveDirection = _movementInput.ReadValue<Vector2>();
			_horizontalInput = moveDirection.x;
			_verticalInput = moveDirection.y;
		}

		private void MovePlayer()
		{
			_moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
	    
			// Ground
			if (_grounded)
				_rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
			// Air
			else if (!_grounded)
				_rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f * airMultiplier), ForceMode.Force);
		}

		private void SpeedCtrl()
		{
			Vector3 flatVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

			if (flatVelocity.magnitude > _moveSpeed)
			{
				Vector3 limitedVelocity = flatVelocity.normalized * _moveSpeed;
				_rb.linearVelocity = new Vector3(limitedVelocity.x, _rb.linearVelocity.y, limitedVelocity.z);
			}
		}

		private void StateHandler()
		{
			// Sprint
			if (_grounded && _sprintInput.IsPressed())
				_moveSpeed = sprintSpeed;
			// Walk
			else if (_grounded)
				_moveSpeed = walkSpeed;
		}
	}
}
