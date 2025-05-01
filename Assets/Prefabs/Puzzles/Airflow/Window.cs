using Objects;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.Airflow
{
    public class Window : ObjectActionable
    {
        private static readonly int ClosedAnimationBoolean = Animator.StringToHash("IsClosed");

        [Header("Window Settings")]
        [SerializeField] private bool startClosed;
        [SerializeField] private Animator windowAnimator;
        
        private readonly NetworkVariable<bool> _isClosed = new NetworkVariable<bool>(false);
        public bool IsClosed => _isClosed.Value;

        public void Start()
        {
            if (IsServer)
                _isClosed.Value = startClosed;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _isClosed.OnValueChanged += UpdatePosition;
        }

        private void UpdatePosition(bool wasClosed, bool isClosed)
        {
            windowAnimator.SetBool(ClosedAnimationBoolean, isClosed);
            AirflowGate.Singleton.CheckWin();
        }
        
        protected override void Action() => ChangeWindowPositionServerRpc(!windowAnimator.GetBool(ClosedAnimationBoolean));

        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void ChangeWindowPositionServerRpc(bool isClosed)
        {
            _isClosed.Value = isClosed;
        }
    }
}