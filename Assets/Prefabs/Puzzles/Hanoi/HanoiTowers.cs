using UnityEngine;
using UnityEngine.Events;

public class HanoiTowers : MonoBehaviour
{
    [Header("Balls")]
    [SerializeField] private GameObject bottomBall;
    [SerializeField] private GameObject middleBall;
    [SerializeField] private GameObject topBall;

    [Header("Colliders")]
    [SerializeField] private GameObject detectorBL;
    [SerializeField] private GameObject detectorML;
    [SerializeField] private GameObject detectorTL;
    [SerializeField] private GameObject detectorBM;
    [SerializeField] private GameObject detectorMM;
    [SerializeField] private GameObject detectorTM;
    [SerializeField] private GameObject detectorBR;
    [SerializeField] private GameObject detectorMR;
    [SerializeField] private GameObject detectorTR;

    private UnityEvent<GameObject, GameObject> _ballEnterBoxEvent;
    
    private HanoiBall _bBall;
    private HanoiBall _mBall;
    private HanoiBall _tBall;
    
    private HanoiCollider _colliderBL;
    private HanoiCollider _colliderML;
    private HanoiCollider _colliderTL;
    private HanoiCollider _colliderBM;
    private HanoiCollider _colliderMM;
    private HanoiCollider _colliderTM;
    private HanoiCollider _colliderBR;
    private HanoiCollider _colliderMR;
    private HanoiCollider _colliderTR;
    
    private void Awake()        // When script gets loaded
    {
    }

    private void Start()        // When game gets loaded
    {
        // Get ball objects
        _bBall = new HanoiBall(bottomBall, 2);
        _mBall = new HanoiBall(middleBall, 1);
        _tBall = new HanoiBall(topBall, 0);
        
        // Get collision boxes
        _colliderBL = new HanoiCollider(detectorBL, 0, _ballEnterBoxEvent);
        _colliderML = new HanoiCollider(detectorML, 1, _ballEnterBoxEvent);
        _colliderTL = new HanoiCollider(detectorTL, 2, _ballEnterBoxEvent);
        _colliderBM = new HanoiCollider(detectorBM, 0, _ballEnterBoxEvent);
        _colliderMM = new HanoiCollider(detectorMM, 1, _ballEnterBoxEvent);
        _colliderTM = new HanoiCollider(detectorTM, 2, _ballEnterBoxEvent);
        _colliderBR = new HanoiCollider(detectorBR, 0, _ballEnterBoxEvent);
        _colliderMR = new HanoiCollider(detectorMR, 1, _ballEnterBoxEvent);
        _colliderTR = new HanoiCollider(detectorTR, 2, _ballEnterBoxEvent);
        // Reset ball positions
        _bBall.Ball.transform.localPosition = new Vector3(2.5f, -1.5f, 1.25f);
        _mBall.Ball.transform.localPosition = new Vector3(2.5f, 0.25f, 1.25f);
        _tBall.Ball.transform.localPosition = new Vector3(2.5f, 2f, 1.25f);
        
        // Add event listeners
        _ballEnterBoxEvent = new UnityEvent<GameObject, GameObject>();
        _ballEnterBoxEvent.AddListener(OnBallEnterBox);
    }

    private void OnBallEnterBox(GameObject box, GameObject ball)
    {
        Debug.Log($"{ball.name} entered {box.name}");
    }
}
