using UnityEngine;

public class AIDirector : MonoBehaviour
{
    private AIStateWandering _aiStateWandering;
    private AIStateBait _aiStateBait;

    [SerializeField] private GameObject _bait;
    
    void Awake()
    {
        _aiStateWandering = GetComponent<AIStateWandering>();
        _aiStateBait = GetComponent<AIStateBait>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _aiStateBait.StateUpdate(_bait.transform.position);
    }
}
