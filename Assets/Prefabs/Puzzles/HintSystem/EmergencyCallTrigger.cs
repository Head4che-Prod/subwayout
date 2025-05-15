using System.Collections;
using Hints;
using Objects;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.HintSystem
{
    public class EmergencyCallTrigger : OffstageNetworkBehaviour, IObjectActionable
    {
        private Animator _triggerAnimator;
        private static readonly int PullTrigger = Animator.StringToHash("TriggerDown");
        private static readonly int InsertTrigger = Animator.StringToHash("InsertTrigger");
        private AudioSource _source;
        private readonly NetworkVariable<bool> _cooldownFinished = new NetworkVariable<bool>(true);

        public void Start()
        {
            _triggerAnimator = GetComponent<Animator>();
        }

        protected override void ChangeActivationState(bool oldValue, bool newValue)
        {
            base.ChangeActivationState(oldValue, newValue);
            if (newValue)
            {
                _triggerAnimator.SetTrigger(InsertTrigger);
                _source = GetComponent<AudioSource>();
                VoiceLine.LoadVoiceLines();
                Hints.HintSystem.EnableHints("BlackboxLocation");
            }
        }
        
        public void Activate()
        {
            SetStageStateServerRpc(true);
        }
        
        public void Action()
        {
            if (_cooldownFinished.Value)
            {
                PlayVoiceLinesServerRpc();
            }
        }
        
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void PlayVoiceLinesServerRpc()
        {
            string line = Hints.HintSystem.GetRandomVoiceLine();
            PlayVoiceLinesClientRpc(line);
            _cooldownFinished.Value = false;
            StartCoroutine(TriggerCooldown(Hints.HintSystem.HintIndex[line].Duration));
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void PlayVoiceLinesClientRpc(string line)
        {
            _triggerAnimator.SetTrigger(PullTrigger);
            _source.clip = Hints.HintSystem.HintIndex[line].VoiceLine;
            _source.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ResetTriggerClientRpc()
        {
            _triggerAnimator.ResetTrigger(PullTrigger);
        }

        private IEnumerator TriggerCooldown(float duration)
        {
            yield return new WaitForSeconds(duration);
            _cooldownFinished.Value = true;
            ResetTriggerClientRpc();
        }
    }
}