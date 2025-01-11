using UnityEngine;

public class HanoiBall
{
    public GameObject Ball { get; }
    public Collider Collider { get; }
    public readonly int Weight;
    
    public HanoiCollider currentPosition = null;

    public HanoiBall(GameObject ball, int ballWeight)
    {
        Ball = ball;
        Collider = ball.GetComponent<Collider>();
        Weight = ballWeight;
    }
}