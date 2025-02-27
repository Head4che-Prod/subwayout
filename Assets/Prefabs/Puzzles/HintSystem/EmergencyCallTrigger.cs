using Objects;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.HintSystem
{
    public class EmergencyCallTrigger : ObjectActionable, IOffStage
    {
        public NetworkVariable<bool> IsOffStage { get; } = new NetworkVariable<bool>(false);
        public MeshRenderer Renderer { get; set; }
        public MeshCollider Collider { get; set; }
        
        private AudioSource _source;

        public void Awake()
        {
            Renderer = GetComponent<MeshRenderer>();
            Collider = GetComponent<MeshCollider>();
        }

        public void ChangeActivationState(bool oldValue, bool newValue)
        {
            Renderer.enabled = newValue;
            Collider.enabled = newValue;
            if (newValue)
            {
                // Play animation
                _source = GetComponent<AudioSource>();
                VoiceLine.LoadVoiceLines();
            }
        }
        public void OnEnable()
        {
            IsOffStage.OnValueChanged += ChangeActivationState;
        }

        public void OnDisable()
        {
            IsOffStage.OnValueChanged -= ChangeActivationState;
        }
        public override void OnNetworkSpawn()
        {
            ChangeActivationState(false, false);
        }

        public void Activate()
        {
            (this as IOffStage).SetStageStateServerRPC(true);
        }
        
        protected override void Action(PlayerObject _)
        {
            Debug.Log("Hint");
            _source.clip = PuzzleHint.GetRandomVoiceLine();
            _source.Play();
        }

    }
}