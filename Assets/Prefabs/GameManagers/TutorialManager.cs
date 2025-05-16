using System;
using System.Collections;
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

        private NetworkVariable<TutorialState> _state = new ();
        
        public TutorialState State
        {
            get => _state.Value;
            set
            {
                Debug.Log($"TutorialManager/Local: {value}");
                UpdateStateServerRpc(value);
                UpdateStateClientRpc(value);
            }
        }
        
        public static bool Exists => _instance != null;
        public static bool CanBeChanged => Exists &&
                                           ((Instance.State == TutorialState.TrainStopped && 
                                             Instance.tunnelAnimator.GetCurrentAnimatorStateInfo(0).IsName("TunnelOnBoarding")) 
                                         || (Instance.State == TutorialState.TrainMoving && 
                                             Instance.tunnelAnimator.GetCurrentAnimatorStateInfo(0).IsName("TunnelMove")));
        
        /// <summary>
        /// This method ask the server to update the state of the puzzle.
        /// Used to perform network-authority based actions.
        /// </summary>
        /// <param name="newState">New state that must be applied.</param>
        /// <exception cref="ArgumentOutOfRangeException">The given state doesn't exist.</exception>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void UpdateStateServerRpc(TutorialState newState)
        {
            Debug.Log($"TutorialManager/Server: {newState}");
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
        
        /// <summary>
        /// This method ask all clients to apply changes related to new state.
        /// Used to perform graphic and UI based actions.
        /// </summary>
        /// <param name="newState">New state that must be applied.</param>
        /// <exception cref="ArgumentOutOfRangeException">The given state doesn't exist.</exception>
        [Rpc(SendTo.Everyone, RequireOwnership = false)]
        private void UpdateStateClientRpc(TutorialState newState)
        {
            Debug.Log($"TutorialManager/Client: {newState}");
            switch (newState)
            {
                case TutorialState.TrainStopped:
                    tunnelAnimator.ResetTrigger(Animator.StringToHash("Move"));
                    tunnelAnimator.SetTrigger(Animator.StringToHash("Stop"));
                    break;

                case TutorialState.TrainMoving:
                    tunnelAnimator.ResetTrigger(Animator.StringToHash("Stop"));
                    tunnelAnimator.SetTrigger(Animator.StringToHash("Move"));
                    triggerOutline.enabled = true;
                    break;

                case TutorialState.HintSystemUnlocked:
                    StopAllCoroutines();
                    tunnelAnimator.ResetTrigger(Animator.StringToHash("Stop"));
                    tunnelAnimator.SetTrigger(Animator.StringToHash("Move"));
                    Destroy(this);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        private void Awake()
        {
            Instance = this;
        }
        
        public override void OnDestroy()
        {
            if (_instance == this) _instance = null;
            base.OnDestroy();
        }

        private void Start()
        {
            StartCoroutine(Starter());
        }

        private IEnumerator Starter()
        {
            yield return new WaitForSeconds(15);
            State = TutorialState.TrainStopped;
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