using Objects;
using Hints;
using Prefabs.Puzzles.DoorCode;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.PlayableCar.Animation
{
    public class DoorAnim : NetworkBehaviour, IObjectActionable
    {
        private static readonly int OpenCabinDoor = Animator.StringToHash("OpenCabinDoor");
        [SerializeField] private Animator animDoor;
        public void Action()
        {
            if (Digicode.Active && Digicode.CanDoorOpen)
                OpenDoorRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void OpenDoorRpc() {
            animDoor.SetBool(OpenCabinDoor, true);
            Digicode.Active = false;
            HintSystem.EnableHints(Hint.Hanoi);
            HintSystem.DisableHints(Hint.CodeUnlocks);
        }
    }
}
