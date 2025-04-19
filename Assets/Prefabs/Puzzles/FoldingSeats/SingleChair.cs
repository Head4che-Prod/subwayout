using System;
using Objects;
using UnityEngine;
using Prefabs.Player;

namespace Prefabs.Puzzles.FoldingSeats
{
    public class SingleChair : ObjectActionable
    {
        private Animator _chairAnimator;
        private static readonly int ChairUp = Animator.StringToHash("activateUp");
        private static readonly int ChairDown = Animator.StringToHash("ChairDown");

        public void Start()
        {
            _chairAnimator = GetComponent<Animator>();
        }
        
        protected override void Action(PlayerObject player)
        {
            Debug.Log("has touched the seat");
            _chairAnimator.SetBool(ChairUp, !_chairAnimator.GetBool(ChairUp));
        }
    }
    

}
