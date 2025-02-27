using Objects;
using Prefabs.Player;
using UnityEngine;

namespace Prefabs.Puzzles.HintSystem
{
    public class EmergencyCallTrigger : ObjectActionable
    {
        private AudioSource _source;
        
        protected override void Action(PlayerObject _)
        {
            Debug.Log("Hint");
            _source.clip = PuzzleHint.GetRandomVoiceLine();
            _source.Play();
        }

        public void Activate()
        {
            _source = GetComponent<AudioSource>();
            VoiceLine.LoadVoiceLines();
            gameObject.SetActive(true);
            // Play animation
        }
    }
}