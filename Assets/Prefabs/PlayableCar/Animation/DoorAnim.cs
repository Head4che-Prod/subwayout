using Objects;
using Prefabs.Player;
using UnityEngine;

public class DoorAnim : ObjectActionable
{
    private static readonly int OpenCabinDoor = Animator.StringToHash("OpenCabinDoor");
    [SerializeField] private Animator _animDoor;
    protected override void Action(PlayerObject player)
    {
        Debug.Log("Closed Door");
        _animDoor.SetBool(OpenCabinDoor, !_animDoor.GetBool(OpenCabinDoor));
    }
}
