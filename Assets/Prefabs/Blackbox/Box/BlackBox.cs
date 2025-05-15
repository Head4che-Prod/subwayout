using System.Collections;
using Objects;
using Prefabs.Blackbox.Sticker;
using Prefabs.GameManagers;
using Prefabs.Player;
using Prefabs.Player.PlayerUI.DebugConsole;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Prefabs.Blackbox.Box
{
    public class BlackBox : NetworkBehaviour
    {
        private enum State
        {
            Hidden,
            PullingOut,
            PulledOut
        }

        [SerializeField] private BlackBoxLid lid;
        [SerializeField] private GameObject wholeSticker;
        [SerializeField] private Transform contentsLocation;
        [SerializeField] private NetworkObject contents;
        private StickerGrabbable GrabbableSticker => StickerGrabbable.Singleton;

        private Animator _slideAnimator;
        private State _state;
        private bool _stickersCombined = false;

        private static BlackBox _singleton;

        public static BlackBox Singleton
        {
            get
            {
                if (_singleton != null)
                    return _singleton;
                Debug.LogError("Black box singleton not set");
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
            _state = State.Hidden;
            wholeSticker.SetActive(false);
            DebugConsole.AddCommand("openBlackBox", Open);
        }

        public void Action()
        {
            if (_state == State.Hidden)
                PullOutClientRpc();
            else if (_state == State.PulledOut
                     && !_stickersCombined
                     && PlayerInteract.LocalPlayerInteract.GrabbedObject as StickerGrabbable == GrabbableSticker)
                AssembleStickersClientRpc();
        }

        /// <summary>
        /// Requests clients to pull out the black box.
        /// </summary>
        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void PullOutClientRpc() => StartCoroutine(RunOpenSequence());

        /// <summary>
        /// Plays the opening sequence and changes states accordingly. 
        /// </summary>
        private IEnumerator RunOpenSequence()
        {
            // Only ever called once, no need to hash.
            _state = State.PullingOut;
            _slideAnimator.SetTrigger("slideBox");
            yield return new WaitForSeconds(1.5f);
            _state = State.PulledOut;
        }

        /// <summary>
        /// Combines the two parts of the sticker, removing the grabbable part from the game.
        /// </summary>
        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void AssembleStickersClientRpc()
        {
            _stickersCombined = true;
            wholeSticker.SetActive(true);
            ObjectPositionManager.ForgetResettableObjectClientRpc(GrabbableSticker);
            ObjectHighlightManager.ForgetHighlightableObject(GrabbableSticker.NetworkObjectId);
            GrabbableSticker.Drop();
            GrabbableSticker.gameObject.SetActive(false);
        }

        /// <summary>
        /// Opens the box's lid as soon as it gets pulled out, and spawn its contents.
        /// </summary>
        public void Open() => StartCoroutine(OpenBoxWhenAble());

        /// <summary>
        /// Waits for the box to get pulled out before opening the lid
        /// </summary>
        private IEnumerator OpenBoxWhenAble()
        {
            while (_state != State.PulledOut)
                yield return null;
            if (IsServer)
                Instantiate(contents, contentsLocation).Spawn();
            lid.RaiseLid();
        }
    }
}