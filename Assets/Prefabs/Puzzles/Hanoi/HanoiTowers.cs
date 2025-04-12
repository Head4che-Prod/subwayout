using System.Collections;
using Prefabs.Player.PlayerUI.DebugConsole;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiTowers : NetworkBehaviour        // Only one should exist AT ALL TIMES
    {
        /// <summary>
        /// The instance of the game currently loaded.
        /// </summary>
        public static HanoiTowers Instance { get; private set; }
        
        /// <summary>
        /// Whether the puzzle should render debug tools.
        /// </summary>
        public bool IsInDebugMode { get; private set; }
        
        /// <summary>
        /// Whether a player is currently using the puzzle.
        /// </summary>
        public NetworkVariable<bool> InUse { get; private set; }
        /// <summary>
        /// Internal grid the game uses to store the positions of the balls.
        /// </summary>
        public readonly HanoiHitbox[,] ColliderGrid = new HanoiHitbox[3,3];
        
        private float _ti;
        private bool _gameWon;

        /// <summary>
        /// Toggle debug visuals on and off.
        /// </summary>
        private void ToggleDebug()
        {
            IsInDebugMode = !IsInDebugMode;
            DebugConsole.Singleton.Log($"Hanoi debug mode {(IsInDebugMode ? "activated" : "deactivated")}.");
        }
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start() // When game gets loaded
        {
            InUse = new NetworkVariable<bool>(false);
            
            _ti = 0f;
            _gameWon = false;
            
            IsInDebugMode = false;
            DebugConsole.AddCommand("hanoiToggleDebug", ToggleDebug);
            DebugConsole.AddCommand("hanoiGrid", () => DebugConsole.Singleton.Log(DebugGrid()));
        }
        
        private void FixedUpdate() {
            if (!_gameWon)
                _ti = Time.time;
            else if (Time.time - _ti > 3)
            {
                Debug.Log("Game won!");
                NetworkManager.Singleton.Shutdown();
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/HomeMenu", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }

        /// <summary>
        /// Updates the internal positions of the balls in the puzzle.
        /// </summary>
        /// <param name="ball"><see cref="HanoiBall"/> that entered the hitbox.</param>
        /// <param name="box"><see cref="HanoiHitbox"/> that was entered.</param>
        public void RepositionBall(HanoiBall ball, HanoiHitbox box)
        {
            // Debug.Log($"{ballObject.name} entered {box.gameObject.name}");
            if (box.containedBall == null
                && (box.height == 0 ||
                    Instance.ColliderGrid[box.bar, box.height - 1].containedBall?.weight > ball.weight))
            {
                //Debug.Log($"Put ball {ballObject.name} at position ({box.Bar}, {box.Height}){(box.Height == 0 ? "" : $" on top of {HanoiCollider.ColliderGrid[box.Bar, box.Height - 1].ContainedBall.gameObject.name}")}.");
                HanoiHitbox.RemoveBall(ball);
                box.containedBall = ball;
            }

            _gameWon = true;
            // Check win condition
            for (int layer = 0; layer < 3; layer++)
                if (Instance.ColliderGrid[2, layer].containedBall?.weight != 2 - layer)
                    _gameWon = false;
        }

        /// <summary>
        /// Resets the balls' internal positions to their initial state.
        /// </summary>
        public void ResetPositions()
        {
            for (int bar = 0; bar < 3; bar++)
            for (int level = 0; level  < 3; level++)
            {
                HanoiBall ball = ColliderGrid[bar, level].containedBall;
                if (ball != null)
                {
                    ColliderGrid[bar, level].containedBall = null;
                    ColliderGrid[0, 2 - ball.weight].containedBall = ball;
                }
            }
        }
        
        /// <summary>
        /// Table representation of the internal state of the game.
        /// </summary>
        private string DebugGrid()
        {
            return "<mspace=0.55em>|------------|------------|------------|\n" +
                   $"|{ColliderGrid[0, 2].containedBall?.gameObject.name,12}|{ColliderGrid[1, 2].containedBall?.gameObject.name,12}|{ColliderGrid[2, 2].containedBall?.gameObject.name,12}|\n" +
                   "|------------|------------|------------|\n" +
                   $"|{ColliderGrid[0, 1].containedBall?.gameObject.name,12}|{ColliderGrid[1, 1].containedBall?.gameObject.name,12}|{ColliderGrid[2, 1].containedBall?.gameObject.name,12}|\n" +
                   "|------------|------------|------------|\n" +
                   $"|{ColliderGrid[0, 0].containedBall?.gameObject.name,12}|{ColliderGrid[1, 0].containedBall?.gameObject.name,12}|{ColliderGrid[2, 0].containedBall?.gameObject.name,12}|\n" +
                   "|------------|------------|------------|</mspace>";
        }
    }
}