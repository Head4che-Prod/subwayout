using System.Collections;
using Objects;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.HintSystem
{
    public class EmergencyCallTrigger : ObjectActionable
    {
        private Animator _triggerAnimator;
        private static readonly int PullTrigger = Animator.StringToHash("TriggerDown");
        private static readonly int InsertTrigger = Animator.StringToHash("InsertTrigger");
        private AudioSource _source;
        private NetworkVariable<bool> _cooldownFinished = new NetworkVariable<bool>(true);

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
            }
        }
        
        public void Activate()
        {
            SetStageStateServerRpc(true);
        }
        
        protected override void Action(PlayerObject _)
        {
            if (_cooldownFinished.Value)
            {
                PlayVoiceLinesServerRpc();
            }
        }
        
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void PlayVoiceLinesServerRpc()
        {
            string line = PuzzleHint.GetRandomVoiceLine();
            PlayVoiceLinesClientRpc(line);
            _cooldownFinished.Value = false;
            StartCoroutine(TriggerCooldown(PuzzleHint.HintIndex[line].Duration));
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void PlayVoiceLinesClientRpc(string line)
        {
            _triggerAnimator.SetTrigger(PullTrigger);
            _source.clip = PuzzleHint.HintIndex[line].VoiceLine;
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