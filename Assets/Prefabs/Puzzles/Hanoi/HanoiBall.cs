using System.Collections.Generic;
using UnityEngine;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiBall
    {
        private static readonly Dictionary<GameObject, HanoiBall> RegisteredBalls =
            new Dictionary<GameObject, HanoiBall>();

        public static HanoiBall GetHanoiBall(GameObject ballObject)
        {
            try
            {
                return RegisteredBalls[ballObject];
            }
            catch (KeyNotFoundException)
            {
                throw new HanoiException(
                    $"Collider collided with object '{ballObject.name}' that is not a registered ball");
            }
        }

        public static void AddHanoiBalls(params HanoiBall[] balls)
        {
            foreach (HanoiBall ball in balls)
                if (!RegisteredBalls.TryAdd(ball.Object, ball))
                    Debug.LogWarning($"Ball '{ball.Object.name}' already registered to Hanoi Towers system");
        }

        public GameObject Object { get; }
        public readonly int Weight;
        public Rigidbody Body { get; }

        public HanoiBall(GameObject ball, int ballWeight)
        {
            Object = ball;
            Weight = ballWeight;
            Body = ball.GetComponent<Rigidbody>();

            RegisteredBalls.TryAdd(Object, this);
        }
    }
}