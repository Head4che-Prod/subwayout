using System;
using Unity.Netcode;
using UnityEngine;

namespace Objects
{
    /// <summary>
    /// Class of interactive objects. 
    /// </summary>
    public abstract class ObjectInteractable : NetworkBehaviour
    {
        //[SerializeField] AudioClip soundEffect;
        
        [Header("Offstage system")]
        [SerializeField] protected bool canBeOffStage;
        [SerializeField] protected bool startOnStage;
        private NetworkVariable<bool> IsOffStage { get; } = new NetworkVariable<bool>(false);
        private Renderer Renderer { get; set; }
        private Collider Collider { get; set; }


        public void Awake()
        {
            if (canBeOffStage)
            {
                Renderer = GetComponent<Renderer>();
                Collider = GetComponent<Collider>();
            }
        }
        

        protected virtual void ChangeActivationState(bool oldValue, bool newValue)
        {
            if (canBeOffStage)
            {
                Renderer.enabled = newValue;
                Collider.enabled = newValue;
            }
        }
        
        public override void OnNetworkSpawn()
        {
            ChangeActivationState(startOnStage, startOnStage);
        }
        
        public void OnEnable()
        {
            if (canBeOffStage)
                IsOffStage.OnValueChanged += ChangeActivationState;
        }

        public void OnDisable()
        {
            if (canBeOffStage)
                IsOffStage.OnValueChanged -= ChangeActivationState;
        }
            
        [Rpc(SendTo.Server, RequireOwnership = false)]
        protected void SetStageStateServerRpc(bool state)
        {
            IsOffStage.Value = state;
        }
    }
}