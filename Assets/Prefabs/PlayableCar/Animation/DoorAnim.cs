using Objects;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Prefabs.PlayableCar.Animation
{
    public class DoorAnim : ObjectActionable
    {
        private static readonly int OpenCabinDoor = Animator.StringToHash("OpenCabinDoor");
        [FormerlySerializedAs("_animDoor")] [SerializeField] private Animator animDoor;
        protected override void Action(PlayerObject player)
        {
            Debug.Log("Closed Door");
            OpenDoorRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void OpenDoorRpc() {
            if (!Digicode.CanDoorOpen)
                return;
            
            animDoor.SetBool(OpenCabinDoor, true);
            Digicode.active = false;
        }
    }
}
