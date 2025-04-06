using System;
using Objects;
using UnityEngine;

namespace Prefabs.GameManagers
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Singleton;
        private TutorialState _state;
        // public static event Action<TutorialState> OnStateChange;
        
        [Header("Train")]
        [SerializeField] private Animator tunnelAnimator;
        
        [Header("EmergencyTrigger")]
        [SerializeField] private Transform triggerGrabbableTransform;

        [Header("Visuals")] 
        [SerializeField] private Transform[] lightsTransforms;
        
        public TutorialState State
        {
            get => _state;
            set
            {
                _state = value;

                switch (value)
                {
                    case TutorialState.TrainStopped:
                        break;
                    case TutorialState.TrainMoving:
                        RestartTrain();
                        break;
                    case TutorialState.HintSystemUnlocked:
                        Destroy(this);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            
                // OnStateChange?.Invoke(value);
            }
        }

        private void Awake() => Singleton = this;
        private void Start() => State = TutorialState.TrainStopped;
        
        private void RestartTrain()
        {
            tunnelAnimator.SetBool(Animator.StringToHash("Move"), true);
            HighlightTrigger();
        }

        private void HighlightTrigger() 
            => triggerGrabbableTransform.GetComponent<ObjectOutline>().enabled = true;
    }

    public enum TutorialState
    {
        TrainStopped, // Waiting player to try to open doors
        TrainMoving,
        HintSystemUnlocked
    }
}