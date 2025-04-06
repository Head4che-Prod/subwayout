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
            TutorialManager.Singleton.State = TutorialState.HintSystemUnlocked;
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