using UnityEngine;

public class PlayerSelectionAnimation : MonoBehaviour
{
    private Vector3 _startPosition;
    private Vector3 _midPosition;
    [SerializeField] public Vector3 finalPosition = new Vector3(0, 0, 0); 
    private void Awake()
    {
        _midPosition = finalPosition;
        _startPosition = transform.position; // set to the offscreen pos
        finalPosition = _startPosition;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, finalPosition, 0.03f); //move the object between pos and finalpos
    }

    public void Replace(bool onTheLeft)
    {
        finalPosition = 
            new Vector3((onTheLeft ? 1 : -1) * _startPosition.x, _startPosition.y, _startPosition.z); //when disabled pos returns to the start one (offscreen one)
    }

    public void Show(bool fromTheLeft)
    {
        transform.position = fromTheLeft ? _startPosition: - _startPosition;
        finalPosition = _midPosition;
    }
    
    public void ShowStart()
    {
        transform.position = _midPosition;
        finalPosition = _midPosition;
    }
}
