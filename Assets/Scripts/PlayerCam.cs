using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : NetworkBehaviour
{
    [Header("Sensibility")]
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        Vector2 lookVector2 = InputSystem.actions.FindAction("Gameplay/Look").ReadValue<Vector2>();
        float lookX = lookVector2.x * Time.deltaTime * sensX;
        float lookY = lookVector2.y * Time.deltaTime * sensY;
		
        yRotation += lookX;
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}