using UnityEngine;
using UnityEngine.Events;

public class HanoiCollider
{
    
    public static HanoiCollider[,] colliderGrid = new HanoiCollider[3, 3];
    public readonly int Height;
    public readonly int Bar;
    
    public GameObject Object { get; }
    public Collider Collider { get; }
    
    public UnityEvent<GameObject, HanoiCollider> BallEnterBoxEvent;
    public UnityEvent<GameObject> CollisionEnterEvent;

    public HanoiBall ContainedBall = null;
    
    public HanoiCollider(GameObject detector, int detectorHeight, int detectorBar, UnityEvent<GameObject, HanoiCollider> ballEnterBoxEvent)
    {
        Object = detector;
        Collider = detector.GetComponent<Collider>();
        Height = detectorHeight;
        Bar = detectorBar;
        colliderGrid[Bar, Height] = this;
        
        HanoiHitbox hitbox = detector.GetComponent<HanoiHitbox>();
        
        BallEnterBoxEvent = ballEnterBoxEvent;
        
        CollisionEnterEvent = hitbox.CollisionEnterEvent;
        CollisionEnterEvent.AddListener(OnCollisionEnter);

    }

    private void OnCollisionEnter(GameObject other)
    {
        // Debug.Log($"{Object.name} heard about collision with {other.name}");
        BallEnterBoxEvent?.Invoke(other, this);
    }

    public static void RemoveBall(HanoiBall ball)
    {
        foreach (HanoiCollider collider in colliderGrid)
            if (collider.ContainedBall == ball)
                collider.ContainedBall = null;
    }
}
