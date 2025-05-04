using System;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.GameManagers
{
    public sealed class EndGameManager : MonoBehaviour
    {
        private static EndGameManager _instance;
        
        /// <summary>
        /// Instance of EndGameManager. Must be unique.
        /// </summary>
        public static EndGameManager Instance
        {
            get
            {
                if (_instance == null)
                    Debug.LogWarning("EndGameManager has not been initialized.");
                return _instance;
            }
            set
            {
                if (_instance == null)
                    _instance = value;
                else
                    Debug.LogError("EndGameManager is already initialized.");
            }
        }
        
        [Header("Train")]
        [SerializeField] private Animator tunnelAnimator;
        
        
        private EndGameState _state;

        /// <summary>
        /// Update the current manager state, and run co-routines related to the action performed.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Input EndGameState don't exist.</exception>
        public EndGameState State
        {
            get => _state;
            set
            {
                switch (value)
                {
                    case EndGameState.WaitingHanoi:
                        break;
                    
                    case EndGameState.HanoiResolved:
                        tunnelAnimator.SetBool(Animator.StringToHash("Onboarding"), true);
                        // Waiting for onboard...
                        break;
                    
                    case EndGameState.FinishGame:
                        Destroy(this);
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                _state = value;
            }
        }
        
        private void Awake()
        {
            Instance = this;
            _state = EndGameState.WaitingHanoi;
        }
    }
    
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
}