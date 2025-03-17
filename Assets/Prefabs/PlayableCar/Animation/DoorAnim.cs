using Objects;
using Prefabs.Player;
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
            animDoor.SetBool(OpenCabinDoor, !animDoor.GetBool(OpenCabinDoor));
        }
    }
}
