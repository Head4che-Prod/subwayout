using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPostion;

    // Update is called once per frame
    private void Update()
    {
        transform.position = cameraPostion.position;
    }
}
