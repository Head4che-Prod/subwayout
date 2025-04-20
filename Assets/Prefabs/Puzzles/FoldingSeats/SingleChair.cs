using System;
using Objects;
using UnityEngine;
using Prefabs.Player;
using Unity.Netcode;

namespace Prefabs.Puzzles.FoldingSeats
{
    public class SingleChair : ObjectActionable
    {
        private static readonly int ChairUp = Animator.StringToHash("activateUp");
        private static readonly int ChairDown = Animator.StringToHash("ChairDown");
        [SerializeField] private Animator chairAnimator;
        private NetworkVariable<bool> isUp = new NetworkVariable<bool>(false);

        private void Start()
        {
            isUp.Value = false;
        }

        protected override void Action(PlayerObject player)
        {
            Debug.Log("has touched the seat");
            chairAnimator.SetBool(ChairUp, !chairAnimator.GetBool(ChairUp));
            isUp.Value = chairAnimator.GetBool(ChairUp);
        }

    }
    

}
