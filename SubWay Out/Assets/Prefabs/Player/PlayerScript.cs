using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : NetworkBehaviour
{
    InputAction MovementInput;

    private void Start()
    {
        MovementInput = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector2 HorizontalMoveDirection = MovementInput.ReadValue<Vector2>();

        Vector3 moveDirection = new Vector3(HorizontalMoveDirection.x, 0, HorizontalMoveDirection.y);
        float movementSpeed = 5;

        transform.position += moveDirection * movementSpeed * Time.deltaTime;
    }
}
