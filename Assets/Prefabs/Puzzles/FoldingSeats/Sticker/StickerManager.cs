using Objects;
using Prefabs.GameManagers;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.FoldingSeats.Sticker
{
    public class StickerManager : NetworkBehaviour, IObjectActionable
    {
        [SerializeField] private GameObject halfSticker;
        [SerializeField] private GameObject  wholeStickerPrefab;
        [SerializeField] private StickerGrabbable sticker;
        private ObjectGrabbable _stickerGrabbable;

        void Start()
        {
            halfSticker.SetActive(true);
            wholeStickerPrefab.SetActive(false);
            _stickerGrabbable = sticker.GetComponent<ObjectGrabbable>();
        }
    
        public void Action()
        {
            if (PlayerInteract.LocalPlayerInteract.GrabbedObject == _stickerGrabbable)
            {
                sticker.Deactivate();
                AssembleStickersRpc();
            }
        }

        /// <summary>
        /// Combines the two parts of the sticker.
        /// </summary>
        [Rpc(SendTo.Everyone)]
        private void AssembleStickersRpc()
        {
            halfSticker.SetActive(false);
            wholeStickerPrefab.SetActive(true);
            ObjectPositionManager.ForgetResettableObjectClientRpc(sticker);
            ObjectHighlightManager.ForgetHighlightableObject(NetworkObjectId);
        }
    }
}
