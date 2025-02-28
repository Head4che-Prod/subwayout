using System.Collections;
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
        
        
        private Animator _triggerAnimator;
        private static readonly int PullTrigger = Animator.StringToHash("PullTrigger");
        private AudioSource _source;
        private NetworkVariable<bool> _cooldownFinished = new NetworkVariable<bool>(true);

        public void Awake()
        {
            Renderer = GetComponent<MeshRenderer>();
            Collider = GetComponent<MeshCollider>();
            _triggerAnimator = GetComponent<Animator>();
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
            (this as IOffStage).SetStageStateRPC(true);
        }
        
        protected override void Action(PlayerObject _)
        {
            if (_cooldownFinished.Value)
            {
                _triggerAnimator.SetTrigger(PullTrigger);
                _source.clip = PuzzleHint.GetRandomVoiceLine();
                _source.Play();
                StartCooldownServerRPC();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void StartCooldownServerRPC()
        {
            _cooldownFinished.Value = false;
            StartCoroutine(TriggerCooldown());
        }

        private IEnumerator TriggerCooldown()
        {
            yield return new WaitForSeconds(20f);
            _cooldownFinished.Value = true;
        }
    }
}