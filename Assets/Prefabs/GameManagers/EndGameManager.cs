using System;
using System.Collections;
using Prefabs.Player;
using Prefabs.Player.PlayerUI.DebugConsole;
using Prefabs.Player.PlayerUI.PauseMenu;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Prefabs.GameManagers
{    
    public enum EndGameState
    {
        /// <summary>
        /// This initial state wait Hanoi puzzle for being resolved.
        /// </summary>
        WaitingHanoi,
        
        /// <summary>
        /// This state must be active when the player have resolved Hanoi puzzle
        /// </summary>
        HanoiResolved,
        
        /// <summary>
        /// This state allow players to open metro's doors.
        /// </summary>
        UnlockDoors,
        
        /// <summary>
        /// This state must be active when the player have open the doors, and execute the endgame cinematic and destroy this.
        /// </summary>
        FinishGame
    }
    public sealed class EndGameManager : NetworkBehaviour
    {
        /// <summary>
        /// Instance of EndGameManager. Must be unique.
        /// </summary>
        private static EndGameManager _instance;
        public static EndGameManager Instance
        {
            get
            {
                if (_instance == null)
                    Debug.LogWarning("EndGameManager has not been initialized.");
                return _instance;
            }
            private set
            {
                if (_instance == null)
                    _instance = value;
                else
                    Debug.LogError("EndGameManager is already initialized.");
            }
        }
        
        [Header("Train")]
        [SerializeField] private Animator tunnelAnimator;

        [SerializeField] private Animator allDoorsAnimator;

        public NetworkVariable<EndGameState> EndGameState { get; } = new NetworkVariable<EndGameState>(GameManagers.EndGameState.WaitingHanoi);

        public EndGameState State
        {
            get => EndGameState.Value;
            set
            {
                UpdateStateServerRpc(value);
                UpdateStateClientRpc(value);
            }
        }
        
        private float _startTime;
        public static bool Exists => _instance != null;
        public static bool CanBeChanged => Exists &&
                                           ((Instance.State is GameManagers.EndGameState.WaitingHanoi && 
                                             Instance.tunnelAnimator.GetCurrentAnimatorStateInfo(0).IsName("TunnelMove")) 
                                         || (Instance.State is GameManagers.EndGameState.HanoiResolved or GameManagers.EndGameState.UnlockDoors or GameManagers.EndGameState.FinishGame && 
                                             Instance.tunnelAnimator.GetCurrentAnimatorStateInfo(0).IsName("TunnelOnBoarding")));
        
        /// <summary>
        /// This method ask the server to update the state of the puzzle.
        /// Used to perform network-authority based actions.
        /// </summary>
        /// <param name="newState">New state that must be applied.</param>
        /// <exception cref="ArgumentOutOfRangeException">The given state doesn't exist.</exception>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void UpdateStateServerRpc(EndGameState newState)
        {
            // Debug.Log($"EndGameManager/Server: {newState}");
            switch (newState)
            {
                case GameManagers.EndGameState.WaitingHanoi:
                    break;
                case GameManagers.EndGameState.HanoiResolved:
                    StartCoroutine(OpenDoorAfterStop());
                    break;
                case GameManagers.EndGameState.UnlockDoors:
                    break;
                case GameManagers.EndGameState.FinishGame:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
            
            if (!this.IsDestroyed()) EndGameState.Value = newState;
        }
        
        /// <summary>
        /// This method ask all clients to apply changes related to new state.
        /// Used to perform graphic and UI based actions.
        /// </summary>
        /// <param name="newState">New state that must be applied.</param>
        /// <exception cref="ArgumentOutOfRangeException">The given state doesn't exist.</exception>
        [Rpc(SendTo.Everyone, RequireOwnership = false)]
        private void UpdateStateClientRpc(EndGameState newState)
        {
            // Debug.Log($"EndGameManager/Client: {newState}");
            switch (newState)
            {
                case GameManagers.EndGameState.WaitingHanoi:
                    break;
                    
                case GameManagers.EndGameState.HanoiResolved:
                    tunnelAnimator.ResetTrigger(Animator.StringToHash("Move"));
                    tunnelAnimator.SetTrigger(Animator.StringToHash("Stop"));
                    break;
                
                case GameManagers.EndGameState.UnlockDoors:
                    allDoorsAnimator.SetTrigger("openAllDoors");
                    break;
                    
                case GameManagers.EndGameState.FinishGame:
                    HomeMenu.HomeMenu.Time = Time.time - _startTime;
                    PlayerObject.LocalPlayer.transform.Find("UI/PauseMenu").GetComponent<PauseMenu>().QuitGame();
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        private IEnumerator OpenDoorAfterStop()
        {
            yield return new WaitForSeconds(5);
            State = GameManagers.EndGameState.UnlockDoors;
        }
        
        private void Awake()
        {
            Instance = this;
            DebugConsole.AddCommand("OpenDoors", () => allDoorsAnimator.SetTrigger("openAllDoors"));
            _startTime = Time.time;
        }

        public override void OnDestroy()
        {
            if (_instance == this) _instance = null;
            base.OnDestroy();
        }
    }

}