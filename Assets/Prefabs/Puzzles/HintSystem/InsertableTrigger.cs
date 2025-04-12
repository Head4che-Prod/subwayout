using Objects;
using Prefabs.GameManagers;
using Unity.Netcode;

namespace Prefabs.Puzzles.HintSystem
{
    public class InsertableTrigger : ObjectGrabbable
    {
        public void Deactivate()
        {
            Drop();
            DisableRPC();
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void DisableRPC()
        {
            ObjectPositionManager.ForgetResettableObjectClientRpc(this);
            ObjectHighlightManager.ForgetHighlightableObjectClientRpc(Outline);
            NetworkObject.Despawn();
        }
    }
}