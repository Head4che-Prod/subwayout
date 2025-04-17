using Objects;
using UnityEngine;
using Prefabs.Player;
using UnityEngine.Serialization;


public class SingleChair : ObjectActionable
{
    private static readonly int activateUp = Animator.StringToHash("activateUp");
    [FormerlySerializedAs("ChairMove")] [SerializeField]private Animator anim;
    protected override void Action(PlayerObject player)
    {
        Debug.Log("Chair anim played ");
        anim.SetBool(activateUp, !anim.GetBool(activateUp));
    }
}
