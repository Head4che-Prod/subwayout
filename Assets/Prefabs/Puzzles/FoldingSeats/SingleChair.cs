using Objects;
using Prefabs.Blackbox.Box;
using UnityEngine;
using Unity.Netcode;

namespace Prefabs.Puzzles.FoldingSeats
{
    public class SingleChair : NetworkBehaviour, IObjectInteractable
    {
        private static readonly int ChairUp = Animator.StringToHash("activateUp");
        [Header("Chair Settings")]
        [SerializeField] private Animator chairAnimator;
        [SerializeField] private bool shouldBeUp = false;
        public bool IsInRightPosition => _isUp.Value != shouldBeUp;
        private readonly NetworkVariable<bool> _isUp = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _isUp.OnValueChanged += UpdatePosition;
        }
        
        /// <summary>
        /// Changes the chair's position and checks the win condition.
        /// </summary>
        /// <param name="_">(unused) If the chair was up.</param>
        /// <param name="newValue">If the chair is now up.</param>
        private void UpdatePosition(bool _, bool newValue)
        {
            chairAnimator.SetBool(ChairUp, newValue);
            if (ChairsManager.Singleton.CheckChairs())
                BlackBox.Singleton.Open();
        }
        public void Action()
            => ChangedServerRpc(!chairAnimator.GetBool(ChairUp));
        
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void ChangedServerRpc(bool isUpValChanged)
        {
            _isUp.Value = isUpValChanged;
        }
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _isUp.OnValueChanged -= UpdatePosition;
        }
    }
}
