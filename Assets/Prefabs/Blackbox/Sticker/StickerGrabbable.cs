using Objects;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Blackbox.Box.Sticker
{
    public class StickerGrabbable : ObjectGrabbable
    {
        
        private static StickerGrabbable _singleton;
        public static StickerGrabbable Singleton
        {
            get
            {
                if (_singleton != null)
                    return _singleton;
                Debug.LogError("Grabbable sticker singleton not set");
                return null;
            }
            private set
            {
                if (_singleton == null)
                    _singleton = value;
                else
                    Debug.LogError("Grabbable sticker singleton already set!");
            }
        }

        public override void Start()
        {
            base.Start();
            Singleton = this;
        }
        
        /// <summary>
        /// Deactivates the sticker on the network.
        /// </summary>
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
