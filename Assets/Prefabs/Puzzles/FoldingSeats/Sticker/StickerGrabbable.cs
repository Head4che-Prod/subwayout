using Objects;
using Unity.Netcode;

namespace Prefabs.Puzzles.FoldingSeats.Sticker
{
    public class StickerGrabbable : ObjectGrabbable
    {
        public void Deactivate()
        {
            Drop();
            DisableStickerRpc();
        }


        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void DisableStickerRpc()
        {
            NetworkObject.Despawn();
        }
    
    
    }
}
