using System;
using Objects;
using UnityEngine;
using Prefabs.Player;
using UnityEngine.Networking;

namespace Prefabs.Puzzles.FoldingSeats
{
    public class SingleChair : ObjectActionable
    {
        private Animator _chairAnimator;
        private static readonly int ChairUp = Animator.StringToHash("ChairUp");
        private static readonly int ChairDown = Animator.StringToHash("ChairDown");

        public void Start()
        {
            _chairAnimator = GetComponent<Animator>();
        }

        protected override void Action(PlayerObject _)
        {
            Debug.Log("seat pressed");
            if (!_chairAnimator.GetBool(ChairUp))
            {
                _chairAnimator.SetBool(ChairUp,true);
            }
            else
            {
                _chairAnimator.SetBool(ChairDown, false);
            }
                
        }
    }
    

}
