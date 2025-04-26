using Objects;
using Prefabs.Player;
using Prefabs.Puzzles.DoorCode;
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
            if (Digicode.CanDoorOpen)
                OpenDoorRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void OpenDoorRpc() {
            animDoor.SetBool(OpenCabinDoor, true);
            Digicode.Active = false;
        }
    }
}
