using Objects;
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
            //gameObject.SetActive(false);
            NetworkObject.Despawn();
        }
    }
}