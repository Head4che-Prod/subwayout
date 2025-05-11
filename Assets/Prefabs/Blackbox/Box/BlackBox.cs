using System.Collections;
using Prefabs.Player.PlayerUI.DebugConsole;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Blackbox.Box
{
    public class BlackBox : NetworkBehaviour
    {
        private enum State
        {
            Closed,
            Opening,
            Open
        }
        
        [SerializeField] private BlackBoxLid lid;
        
        private Animator _slideAnimator;
        private State _state;

        private static BlackBox _singleton;
        public static BlackBox Singleton
        {
            get
            {
                if (_singleton != null)
                    return _singleton;
                Debug.LogError("Black box singleton no set");
                return null;
            }
            private set
            {
                if (_singleton == null)
                    _singleton = value;
                else
                    Debug.LogError("Black box singleton already set!");
            }
        }
        
        public void Start()
        {
            Singleton = this;
            _slideAnimator = GetComponent<Animator>();
            _state = State.Closed;
            DebugConsole.AddCommand("openBlackBox", Open);
        }

        public void Action()
        {
            if (_state == State.Closed)
            {
                Debug.Log("Action");
                PullOutClientRpc();
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void PullOutClientRpc() => StartCoroutine(RunOpenSequence());

        /// <summary>
        /// Plays the opening sequence and changes states accordingly. 
        /// </summary>
        private IEnumerator RunOpenSequence()
        {
            // Only ever called once, no need to hash.
            _state = State.Opening;
            _slideAnimator.SetTrigger("slideBox");
            yield return new WaitForSeconds(1.5f);
            _state = State.Open;
        }

        /// <summary>
        /// Opens the box's lid as soon as it gets pulled out.
        /// </summary>
        public void Open() => StartCoroutine(OpenBoxWhenAble());

        /// <summary>
        /// Waits for the box to get pulled out before opening the lid
        /// </summary>
        private IEnumerator OpenBoxWhenAble()
        {
            while (_state != State.Open)
                yield return null;
            lid.RaiseLid();
        }
    }
}