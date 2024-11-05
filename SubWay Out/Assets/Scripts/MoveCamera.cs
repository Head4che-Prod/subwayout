using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPostion;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position = cameraPostion.position;
    }
}
