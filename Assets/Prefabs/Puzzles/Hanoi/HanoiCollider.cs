using UnityEngine;
using UnityEngine.Events;

public class HanoiCollider
{
    public GameObject Object { get; }
    public Collider Collider { get; }
    public readonly int Height;
    
    public UnityEvent<GameObject, GameObject> BallEnterBoxEvent;
    public UnityEvent<GameObject> CollisionEnterEvent;

    public HanoiCollider(GameObject detector, int detectorHeight, UnityEvent<GameObject, GameObject> ballEnterBoxEvent)
    {
        Object = detector;
        Collider = detector.GetComponent<Collider>();
        Height = detectorHeight;
        
        HanoiHitbox hitbox = detector.GetComponent<HanoiHitbox>();
        CollisionEnterEvent = hitbox.CollisionEnterEvent;

        CollisionEnterEvent.AddListener(OnCollisionEnter);
    }

    private void OnCollisionEnter(GameObject other)
    {
        BallEnterBoxEvent.Invoke(Object, other);
    }
}
