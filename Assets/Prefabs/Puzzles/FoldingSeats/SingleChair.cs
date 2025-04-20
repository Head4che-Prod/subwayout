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
        [SerializeField] private Animator chairAnimator;
        private NetworkVariable<bool> _isUp = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);

        // protected override void Action(PlayerObject player)
        // {
        //     Debug.Log("has touched the seat");
        //     // chairAnimator.SetBool(ChairUp, !chairAnimator.GetBool(ChairUp));
        //     // isUp.Value = chairAnimator.GetBool(ChairUp);
        //
        //     _isUp.Value = !chairAnimator.GetBool(ChairUp);
        //     _isUp.OnValueChanged += AnimateChair;
        // }
        //
        // private void AnimateChair(bool _, bool curr)
        // {
        //     Debug.Log($"[CHAIR] New value: {curr}");
        //     chairAnimator.SetBool(ChairUp, curr);
        // } 

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _isUp.OnValueChanged += OnValueChanged;
        }

        private void OnValueChanged(bool previousValue, bool newValue)
        {
            if (newValue)
            {
                chairAnimator.SetBool(ChairUp, true);
            }
            else
            {
                chairAnimator.SetBool(ChairUp, false);
            }
        }
        protected override void Action(PlayerObject player)
        {
           ChangedServerRpc(!chairAnimator.GetBool(ChairUp));
        }
        
        [ServerRpc]
        private void ChangedServerRpc(bool isUpValChanged)
        {
            _isUp.Value = isUpValChanged;
        }
        
        
        
    }
}
