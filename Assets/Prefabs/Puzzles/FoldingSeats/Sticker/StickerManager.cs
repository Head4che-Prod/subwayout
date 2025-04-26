using Objects;
using Prefabs.Player;
using UnityEngine;

namespace Prefabs.Puzzles.FoldingSeats.Sticker
{
    public class StickerManager : ObjectActionable
    {
        [SerializeField] private GameObject  wholeStickerPrefab;
        [SerializeField] private StickerGrabbable sticker;
        private ObjectGrabbable _stickerGrabbable;

        void Start()
        {
            wholeStickerPrefab.SetActive(false);
            _stickerGrabbable = sticker.GetComponent<ObjectGrabbable>();
        }
    
        protected override void Action(PlayerObject player)
        {
            if (_stickerGrabbable.Owner == player)
            {
                sticker.Deactivate();
                wholeStickerPrefab.SetActive(true);
            }
        }
    }
}
