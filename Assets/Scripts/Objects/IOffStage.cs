using Unity.Netcode;
using UnityEngine;

namespace Objects
{
    public interface IOffStage
    {
        public NetworkVariable<bool> IsOffStage { get; } // = new NetworkVariable<bool>(false);
        public MeshRenderer Renderer { get; }
        public MeshCollider Collider { get; }

        public void Awake();    // Renderer = GetComponent<MeshRenderer>(); Collider = GetComponent<MeshCollider>();

        public void OnEnable(); // IsOffStage.OnValueChanged += ChangeActivationState;

        public void OnDisable();    // IsOffStage.OnValueChanged -= ChangeActivationState;

        public void ChangeActivationState(bool oldValue, bool newValue)
        {
            Renderer.enabled = newValue;
            Collider.enabled = newValue;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetStageStateServerRPC(bool state)
        {
            IsOffStage.Value = state;
        }
    }
}