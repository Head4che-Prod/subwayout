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
                // Play animation
                _source = GetComponent<AudioSource>();
                VoiceLine.LoadVoiceLines();
            }
        }
        
        public void Activate()
        {
            SetStageStateServerRPC(true);
        }
        
        protected override void Action(PlayerObject _)
        {
            if (_cooldownFinished.Value)
            {
                PlayVoiceLinesServerRPC();
            }
        }
        [ServerRpc(RequireOwnership = false)]
        private void PlayVoiceLinesServerRPC()
        {
            PlayVoiceLinesClientRPC(PuzzleHint.GetRandomVoiceLine());
            _cooldownFinished.Value = false;
            StartCoroutine(TriggerCooldown());
        }

        [ClientRpc]
        private void PlayVoiceLinesClientRPC(string line)
        {
            _triggerAnimator.SetTrigger(PullTrigger);
            _source.clip = PuzzleHint.HintIndex[line].VoiceLine;
            _source.Play();
        }

        private IEnumerator TriggerCooldown()
        {
            yield return new WaitForSeconds(20f);
            _cooldownFinished.Value = true;
            _triggerAnimator.ResetTrigger(PullTrigger);
        }
    }
}