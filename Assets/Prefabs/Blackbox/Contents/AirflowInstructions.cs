using Hints;
using Objects;
using Prefabs.GameManagers;
using Unity.Netcode;

namespace Prefabs.Blackbox.Contents
{
    public class AirflowInstructions : ObjectGrabbable
    {
        private bool _pickedUp = false;
        
        public override void Grab()
        {
            if (!_pickedUp)
            {
                BlueCodeFoundRpc();
            }
            base.Grab();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void BlueCodeFoundRpc()
        {
            HintSystem.DisableHints(Hint.BlueCode);
            _pickedUp = true;
            if(IsServer)
                CodesFoundManager.Singleton.BlueCodeFound.Value = true;
        }
    }
}
