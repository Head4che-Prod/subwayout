using System;
using Objects;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Prefabs.GameManagers
{
    public sealed class TutorialManager : NetworkBehaviour
    {
        /// <summary>
        /// Instance of TutorialManager. Must be unique.
        /// </summary>
        private static TutorialManager _instance;
        public static TutorialManager Instance
        {
            get
            {
                if (_instance == null)
                    Debug.LogWarning("TutorialManager has not been initialized.");
                return _instance;
            }
            set
            {
                if (_instance == null)
                    _instance = value;
                else
                    Debug.LogError("TutorialManager is already initialized.");
            }
        }
        
        [Header("Train")]
        [SerializeField] private Animator tunnelAnimator;

        [Header("EmergencyTrigger")]
        [SerializeField] private ObjectOutline triggerOutline;
        
        private NetworkVariable<TutorialState> _state;
        
        public TutorialState State
        {
            get => _state.Value;
            set
            {
                UpdateStateServerRpc(value);
                UpdateStateClientRpc(value);
            }
        }
        
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void UpdateStateServerRpc(TutorialState newState)
        {
            switch (newState)
            {
                case TutorialState.TrainStopped:
                    break;

                case TutorialState.TrainMoving:
                    break;

                case TutorialState.HintSystemUnlocked:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
            
            if (!this.IsDestroyed()) _state.Value = newState;
        }
        
        [Rpc(SendTo.Everyone, RequireOwnership = false)]
        private void UpdateStateClientRpc(TutorialState newState)
        {
            switch (newState)
            {
                case TutorialState.TrainStopped:
                    break;

                case TutorialState.TrainMoving:
                    tunnelAnimator.SetBool(Animator.StringToHash("Move"), true);
                    triggerOutline.enabled = true;
                    break;

                case TutorialState.HintSystemUnlocked:
                    Destroy(this);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        private void Awake()
        {
            Instance = this;
            if (IsHost) _state.Value = TutorialState.TrainStopped;
        }
    }
    
    public enum TutorialState
    {
        /// <summary>
        /// This state is the initial one.
        /// </summary>
        TrainStopped,
        
        /// <summary>
        /// Facultative. This state can be activated when the player tries to open the doors and reboot the trains motor.
        /// </summary>
        TrainMoving,
        
        /// <summary>
        /// This state must be active when the player have activated the hint system, and destroy this.
        /// </summary>
        HintSystemUnlocked
    }
}