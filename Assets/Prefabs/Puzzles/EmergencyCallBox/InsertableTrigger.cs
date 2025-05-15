using System.Collections;
using Objects;
using Prefabs.GameManagers;
using Prefabs.Player;
using Unity.Netcode;

namespace Prefabs.Puzzles.EmergencyCallBox
{
    public class InsertableTrigger : ObjectGrabbable
    {
        /// <summary>
        /// Called on server only. Deactivates the trigger.
        /// <param name="originalClientId">The id of the client that is holding the trigger.</param>
        /// </summary>
        public void Deactivate()
        {
            PlayerInteract.LocalPlayerInteract.GrabbedObject = null;
            DeactivateServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        /// <summary>
        /// Asks the client that was holding the trigger to drop it.
        /// </summary>
        /// <param name="clientId">The id of the client that is holding the trigger.</param>
        [Rpc(SendTo.Server)]
        private void DeactivateServerRpc(ulong clientId)
        {
            GrabbedObjectManager.PlayerDrop(clientId);
            ObjectPositionManager.ForgetResettableObjectClientRpc(this);
            ObjectHighlightManager.ForgetHighlightableObject(NetworkObjectId);
            StartCoroutine(DelayedDestroy());
        }
        
        /// <summary>
        /// Waits one frame before destroying the object.
        /// </summary>
        private IEnumerator DelayedDestroy()
        {
            yield return null;
            NetworkObject.Despawn();
        }
    }
}