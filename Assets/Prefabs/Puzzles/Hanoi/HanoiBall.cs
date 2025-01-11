using System.Collections.Generic;
using UnityEngine;

public class HanoiBall
{
    private static Dictionary<GameObject, HanoiBall> _registeredBalls = new Dictionary<GameObject, HanoiBall>();

    public static HanoiBall GetHanoiBall(GameObject ballObject)
    {
        try
        {
            return _registeredBalls[ballObject];
        }
        catch (KeyNotFoundException)
        {
            throw new HanoiException($"Collider collided with object '{ballObject.name}' that is not a registered ball");
        }
    }

    public static void AddHanoiBalls(params HanoiBall[] balls)
    {
        foreach (HanoiBall ball in balls)
            if (!_registeredBalls.TryAdd(ball.Object, ball))
                Debug.LogWarning($"Ball '{ball.Object.name}' already registered to Hanoi Towers system");
    }
    
    public GameObject Object { get; }
    public Collider Collider { get; }
    public readonly int Weight;
    public HanoiBall(GameObject ball, int ballWeight)
    {
        Object = ball;
        Collider = ball.GetComponent<Collider>();
        Weight = ballWeight;
        
        _registeredBalls.TryAdd(Object, this);
    }
}