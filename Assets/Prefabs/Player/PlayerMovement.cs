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

		[Header("Keybindings")] 
		private InputAction _movementInput;
		private InputAction _sprintInput;

		[Header("Ground Check")]
		public Collider PlayerCollider;
		private float _colliderHeight;
		public LayerMask whatIsGround;
		private bool _grounded;

		private float _horizontalInput;
		private float _verticalInput;

		private PlayerObject _player;
		
		void Start()
		{
			_player = GetComponent<PlayerObject>();
			_player.Rigidbody.freezeRotation = true;
			_movementInput = _player.Input.actions["Move"];
			_sprintInput = _player.Input.actions["Sprint"];

			_colliderHeight = PlayerCollider.bounds.size.y;
		}
    
		void Update()
		{
			_grounded = Physics.Raycast(PlayerCollider.transform.position, Vector3.down, _colliderHeight * 0.5f + 0.2f, whatIsGround);
			Debug.DrawRay(PlayerCollider.transform.position, Vector3.down, Color.blue,_colliderHeight * 0.5f + 0.2f);
	    
			ProcessInputs();
			SpeedCtrl();
			StateHandler();
	    
			if (_grounded)
				_player.Rigidbody.linearDamping = groundDrag;
			else
				_player.Rigidbody.linearDamping = 0;
		}

		void FixedUpdate()
		{
			MovePlayer();
		}

		private void ProcessInputs()
		{
			Vector2 moveDirection = _movementInput.ReadValue<Vector2>();
			_horizontalInput = moveDirection.x;
			_verticalInput = moveDirection.y;
		}

		private void MovePlayer()
		{
			_moveDirection = transform.forward * _verticalInput + transform.right * _horizontalInput;
	    
			// Ground
			if (_grounded)
				_player.Rigidbody.AddForce(_moveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
			// Air
			else if (!_grounded)
				_player.Rigidbody.AddForce(_moveDirection.normalized * (_moveSpeed * 10f * airMultiplier), ForceMode.Force);
		}

		private void SpeedCtrl()
		{
			Vector3 flatVelocity = new Vector3(_player.Rigidbody.linearVelocity.x, 0f, _player.Rigidbody.linearVelocity.z);

			if (flatVelocity.magnitude > _moveSpeed)
			{
				Vector3 limitedVelocity = flatVelocity.normalized * _moveSpeed;
				_player.Rigidbody.linearVelocity = new Vector3(limitedVelocity.x, _player.Rigidbody.linearVelocity.y, limitedVelocity.z);
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
