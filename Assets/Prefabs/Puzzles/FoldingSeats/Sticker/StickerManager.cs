using Objects;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.FoldingSeats.Sticker
{
    public class StickerManager : ObjectActionable
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
    
        protected override void Action(PlayerObject player)
        {
            if (_stickerGrabbable.Owner == player)
            {
                DisableStickerRpc();
            }
        }

        [Rpc(SendTo.Everyone)]
        public void DisableStickerRpc()
        {
            sticker.Deactivate();
            halfSticker.SetActive(false);
            wholeStickerPrefab.SetActive(true);
        }
    }
}
