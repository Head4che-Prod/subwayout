using UnityEngine;

public class PlayerSelectionAnimation : MonoBehaviour
{
    private Vector3 _startPosition;
    [SerializeField] private Vector3 finalPosition; 
    private void Awake()
    {
        _startPosition = transform.position; // set to the offscreen pos
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, finalPosition, 0.03f); //move the object between pos and finalpos
    }

    private void OnDisable()
    {
        transform.position = _startPosition; //when disabled pos returns to the start one (offscreen one)
    }
    
    
    
}
