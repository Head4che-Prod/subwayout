using Objects;
using UnityEngine;
using Prefabs.Player;
using UnityEngine.Serialization;


public class SingleChair : ObjectActionable
{
    private static readonly int chairMove = Animator.StringToHash("ChairMove");
    [FormerlySerializedAs("ChairMove")] [SerializeField]private Animator anim;
    protected override void Action(PlayerObject player)
    {
        anim.SetBool(chairMove, !anim.GetBool(chairMove));
    }
}
