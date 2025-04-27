using Objects;
using Prefabs.GameManagers;
using Unity.Netcode;

namespace Prefabs.Puzzles.HintSystem
{
    public class InsertableTrigger : ObjectGrabbable
    {
        /// <summary>
        /// Called on server only. Deactivates the trigger.
        /// <param name="originalClientId">The id of the client that is holding the trigger.</param>
        /// </summary>
        public void Deactivate(ulong originalClientId)
        {
            DropRpc(originalClientId);
            ObjectPositionManager.ForgetResettableObjectClientRpc(this);
            ObjectHighlightManager.ForgetHighlightableObjectClientRpc(Outline);
            NetworkObject.Despawn();
        }

        /// <summary>
        /// Asks the client that was holding the trigger to drop it.
        /// </summary>
        /// <param name="clientId">The id of the client that is holding the trigger.</param>
        [Rpc(SendTo.ClientsAndHost)]
        private void DropRpc(ulong clientId)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
                Drop();
        }
    }
}