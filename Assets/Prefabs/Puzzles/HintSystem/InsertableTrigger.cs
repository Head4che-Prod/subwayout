using Objects;
using Unity.Netcode;

namespace Prefabs.Puzzles.HintSystem
{
    public class InsertableTrigger : ObjectGrabbable
    {
        public void Deactivate()
        {
            Drop();
            DisableServerRPC();
        }

        [ServerRpc(RequireOwnership = false)]
        private void DisableServerRPC()
        {
            //gameObject.SetActive(false);
            NetworkObject.Despawn();
        }
    }
}