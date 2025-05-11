using Objects;
using Unity.Netcode;

namespace Prefabs.Blackbox.Box.Sticker
{
    public class StickerGrabbable : ObjectGrabbable
    {

        public ObjectOutline StickerOutline => Outline;
        public void Deactivate()
        {
            Drop();
            DisableStickerRpc();
        }

        /// <summary>
        /// Destroys the grabbable sticker, removing it from the game managers.
        /// </summary>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void DisableStickerRpc()
        {
            NetworkObject.Despawn();
        }
    
    
    }
}
