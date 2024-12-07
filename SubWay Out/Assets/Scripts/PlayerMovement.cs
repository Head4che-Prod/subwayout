using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Movement")] public float moveSpeed;

	public float groundDrag;

	public Transform orientation;

	float horizontalInput;
	float verticalInput;

	private Vector3 moveDirection;
	
	Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    rb = GetComponent<Rigidbody>();
	    rb.freezeRotation = true;
    }
    
    void Update()
    {
	    MyInput();
	    rb.drag	= groundDrag;
    }

    void FixedUpdate()
    {
	    MovePlayer();
    }

    private void MyInput()
    {
	    horizontalInput = Input.GetAxisRaw("Horizontal");
	    verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
	    moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
	    rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    // Update is called once per frame
    
}
