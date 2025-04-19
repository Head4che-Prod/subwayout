using System;
using Objects;
using UnityEngine;
using Prefabs.Player;
using UnityEngine.Serialization;

namespace Prefabs.Puzzles.FoldingSeats
{
    public class SingleChair : ObjectActionable
    {
        private static readonly int ChairUp = Animator.StringToHash("activateUp");
        private static readonly int ChairDown = Animator.StringToHash("ChairDown");
        [SerializeField] private Animator chairAnimator;
        
        protected override void Action(PlayerObject player)
        {
            Debug.Log("has touched the seat");
            chairAnimator.SetBool(ChairUp, !chairAnimator.GetBool(ChairUp));
        }
    }
    

}
