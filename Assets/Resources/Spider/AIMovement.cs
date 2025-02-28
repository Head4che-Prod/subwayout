using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    [SerializeField] private float navMeshSearchRange = 1.0f;
    [SerializeField] private string walkAnimation = "isWalking";
    [SerializeField] private string runAnimation = "isRunning";
    [SerializeField] private float walkSpeed = 1.0f;
    [SerializeField] private float runSpeed = 2.3f;

    private int _walkAnimationId;
    private int _runAnimationId;

    private Animator _animator;

    private NavMeshAgent _agent;
    private NavMeshHit _hit;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _walkAnimationId = Animator.StringToHash(walkAnimation);
        _runAnimationId = Animator.StringToHash(runAnimation);
    }

    public void MoveTo(Vector3 position)
    {
        _agent.speed = walkSpeed;
        _agent.SetDestination(position);
    }

    public void MoveToRandom(Vector3 center, float range)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;

        if (NavMesh.SamplePosition(randomPoint, out _hit, navMeshSearchRange, NavMesh.AllAreas))
            MoveTo(_hit.position);
    }

    public void RunTo(Vector3 position)
    {
        _agent.speed = runSpeed;
        _agent.SetDestination(position);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_animator)
        {
            if (_agent.remainingDistance < 0.1f)
            {
                _animator.SetBool(_walkAnimationId, false);
                _animator.SetBool(_runAnimationId, false);
            }
            else
            {
                if (_agent.speed <= walkSpeed)
                {
                    _animator.SetBool(_walkAnimationId, true);
                    _animator.SetBool(_runAnimationId, false);
                }
                else
                {
                    _animator.SetBool(_walkAnimationId, false);
                    _animator.SetBool(_runAnimationId, true);
                }
            }
        }
    }
}