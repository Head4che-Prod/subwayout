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

        private void OnValueChanged(bool _, bool newValue)
            => chairAnimator.SetBool(ChairUp, newValue);
        protected override void Action(PlayerObject player)
            => ChangedServerRpc(!chairAnimator.GetBool(ChairUp));
        
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void ChangedServerRpc(bool isUpValChanged)
        {
            _isUp.Value = isUpValChanged;
        }
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _isUp.OnValueChanged -= OnValueChanged;
        }
    }
}
