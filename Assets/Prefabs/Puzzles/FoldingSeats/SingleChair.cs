using System;
using Objects;
using UnityEngine;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine.Serialization;

namespace Prefabs.Puzzles.FoldingSeats
{
    public class SingleChair : ObjectActionable
    {
        private static readonly int ChairUp = Animator.StringToHash("activateUp");
        [SerializeField] private Animator chairAnimator;
        public NetworkVariable<bool> isUp = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            isUp.OnValueChanged += OnValueChanged;
        }

        private void OnValueChanged(bool _, bool newValue)
        {
            chairAnimator.SetBool(ChairUp, newValue);
            ChairsManager.Singleton.CheckChairs(); // we call CheckChairs here so that when only one value changes we check, no need to check at every frame
        }
        protected override void Action(PlayerObject player)
            => ChangedServerRpc(!chairAnimator.GetBool(ChairUp));
        
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void ChangedServerRpc(bool isUpValChanged)
        {
            isUp.Value = isUpValChanged;
        }
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            isUp.OnValueChanged -= OnValueChanged;
        }
    }
}
