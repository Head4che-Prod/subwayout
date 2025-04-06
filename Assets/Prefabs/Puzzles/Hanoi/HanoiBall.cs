using System.Collections.Generic;
using UnityEngine;

namespace Prefabs.Puzzles.Hanoi
{
    /// <summary>
    /// Internal representation of a ball, storing its size and position.
    /// </summary>
    public class HanoiBall
    {
        private static readonly Dictionary<GameObject, HanoiBall> RegisteredBalls =
            new Dictionary<GameObject, HanoiBall>();
        
        /// <summary>
        /// Fetch the HanoiBall that corresponds to the <c>ballObject</c>.
        /// </summary>
        /// <param name="ballObject"><see cref="GameObject"/></param>
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
        
        /// <summary>
        /// Registers HanoiBalls to the system, linking them to their <see cref="GameObject"/>.
        /// </summary>
        /// <param name="balls"><see cref="HanoiBall"/>s to register.</param>
        public static void AddHanoiBalls(params HanoiBall[] balls)
        {
            foreach (HanoiBall ball in balls)
                if (!RegisteredBalls.TryAdd(ball.Object, ball))
                    Debug.LogWarning($"Ball '{ball.Object.name}' already registered to Hanoi Towers system");
        }

        /// <summary>
        /// Object the HanoiBall represents.
        /// </summary>
        public GameObject Object { get; }
        public readonly int Weight;
        
        /// <summary>
        /// HanoiBall constructor.
        /// </summary>
        /// <param name="ball"><see cref="GameObject"/> the HanoiBall represents.</param>
        /// <param name="ballWeight"><see cref="ushort"/> representing how heavy the ball is. Balls can not be placed on lighter ones.</param>
        public HanoiBall(GameObject ball, ushort ballWeight)
        {
            Object = ball;
            Weight = ballWeight;

            RegisteredBalls.TryAdd(Object, this);
        }
    }
}