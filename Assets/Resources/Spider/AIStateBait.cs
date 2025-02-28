using UnityEngine;
using UnityEngine.AI;

public class AIStateBait : MonoBehaviour
{
    [SerializeField] private float stopRange = 3;
    [SerializeField] private string IdleAnimation = "Idle";

    private AIMovement _aiMovement;

    private int _idleAnimationId;
    private Animator _animator;

    private NavMeshAgent _agent;

    void Awake()
    {
        _aiMovement = GetComponent<AIMovement>();
        _agent = GetComponent<NavMeshAgent>();

        _animator = GetComponent<Animator>();
        _idleAnimationId = Animator.StringToHash(IdleAnimation);
    }

    public void StateUpdate(Vector3 targetPosition)
    {
        if (Vector3.Distance(targetPosition, transform.position) >= stopRange)
        {
            _aiMovement.RunTo(targetPosition);
        }
        else
        {
            _aiMovement.RunTo(transform.position);
            _animator.SetBool(_idleAnimationId, true);
            transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
