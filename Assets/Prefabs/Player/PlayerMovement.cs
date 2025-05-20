using Unity.Netcode;
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
		public Collider playerCollider;
		private float _colliderHeight;
		public LayerMask whatIsGround;
		private bool _grounded;

		private float _horizontalInput;
		private float _verticalInput;

		private PlayerObject _player;
		private WalkAnimSync walkScript;
		
		void Start()
		{
			_player = GetComponent<PlayerObject>();
			_player.Rigidbody.freezeRotation = true;
			_movementInput = _player.Input.actions["Move"];
			_sprintInput = _player.Input.actions["Sprint"];

			_colliderHeight = playerCollider.bounds.size.y;
			walkScript = GetComponent<WalkAnimSync>();
		}
    
		private void LateUpdate()
		{
			_grounded = Physics.Raycast(playerCollider.transform.position, Vector3.down, _colliderHeight * 0.5f + 0.2f, whatIsGround);
			Debug.DrawRay(playerCollider.transform.position, Vector3.down, Color.blue,_colliderHeight * 0.5f + 0.2f);
	    
			ProcessInputs();
			SpeedCtrl();
			StateHandler();
			MovePlayer();
	    
			if (_grounded)
				_player.Rigidbody.linearDamping = groundDrag;
			else
				_player.Rigidbody.linearDamping = 0;
		}

		private void ProcessInputs()
		{
			Vector2 moveDirection = _movementInput.ReadValue<Vector2>();
			_horizontalInput = moveDirection.x;
			_verticalInput = moveDirection.y;
			walkScript.CallWalkAnimationRpc( _horizontalInput != 0 || _verticalInput != 0);
		}
		

		private void MovePlayer()
		{
			_moveDirection = transform.forward * _verticalInput + transform.right * _horizontalInput;
	    
			// Ground
			if (_grounded)
				_player.Rigidbody.AddForce(_moveDirection.normalized * (_moveSpeed * 10f * Time.deltaTime / Time.fixedDeltaTime), ForceMode.Force);
			// Air
			else if (!_grounded)
				_player.Rigidbody.AddForce(_moveDirection.normalized * (_moveSpeed * 10f * airMultiplier * Time.deltaTime / Time.fixedDeltaTime), ForceMode.Force);
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
