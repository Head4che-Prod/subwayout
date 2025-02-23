using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : NetworkBehaviour
{
    [Header("Sensibility")]
    public float sensX;
    public float sensY;

    public Transform orientation;

    private float _xRotation;
    private float _yRotation;
    
    private InputAction _lookAction;
    private PlayerObject _player;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _player = GetComponentInParent<PlayerObject>();
        _lookAction = _player.Input.actions["Look"];
    }
    
    void Update()
    {
        Vector2 lookVector2 = _lookAction.ReadValue<Vector2>();
        float lookX = lookVector2.x * Time.deltaTime * sensX;
        float lookY = lookVector2.y * Time.deltaTime * sensY;
		
        _yRotation += lookX;
        _xRotation -= lookY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
    }
}