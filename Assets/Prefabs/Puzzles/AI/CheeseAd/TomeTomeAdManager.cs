using System;
using JetBrains.Annotations;
using Objects;
using Prefabs.GameManagers;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.AI.CheeseAd
{
    public class TomeTomeAdManager : NetworkBehaviour,IObjectActionable
    {
        [CanBeNull] private ObjectGrabbable _keyGrabbable;
        [SerializeField] private GameObject _keyInAd;
        private Animator animator;
        private static readonly int openCheeseAdAnim = Animator.StringToHash("openCheeseAd");
        private readonly NetworkVariable<bool> _isOpen = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
        

        void Start()
        {
            _keyInAd.SetActive(false);
            animator = transform.GetChild(0).GetComponent<Animator>();
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _isOpen.OnValueChanged += UpdatePosition;
        }
        
        
        void Update()
        {
            _keyGrabbable = (ObjectGrabbable)PlayerInteract.LocalPlayerInteract.GrabbedObject;
        }

        public void Action()
        {
            if (_keyGrabbable is not null && _keyGrabbable.name == "keyGrabbable(Clone)")
            {
                DeactivateGrabbedKey();
            }

            if (_keyInAd.activeInHierarchy)
            {
                ChangeAdDoorServerRpc(!animator.GetBool(openCheeseAdAnim));
                Debug.Log("Cheese Ad anim is played");
            }
        }
        
        
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void ChangeAdDoorServerRpc(bool isOpenValChanged)
        {
            _isOpen.Value = isOpenValChanged;
        }
        /// <summary>
        /// Changes the position of the ad's door.
        /// </summary>
        private void UpdatePosition(bool _, bool newValue)
        {
            animator.SetBool(openCheeseAdAnim, newValue);
        }
        
        /// <summary>
        /// Deactivate the grabbed key but activate the 3D model in the ad.
        /// </summary>
        public void DeactivateGrabbedKey()
        {
            _keyGrabbable!.Drop();
            DisableCheeseRpc(_keyGrabbable.NetworkObjectId);
            ActivateKeyInAdRpc();
        }
        /// <summary>
        /// Removes the key that was grabbed.
        /// </summary>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void DisableCheeseRpc(ulong keyGrabbedID)
        {
            ObjectHighlightManager.ForgetHighlightableObject(keyGrabbedID);
            NetworkManager.Singleton.SpawnManager.SpawnedObjects[keyGrabbedID].Despawn();
        }
    
        /// <summary>
        /// Activate the key in the cheese ad.
        /// </summary>
        [Rpc(SendTo.Everyone)]
        private void ActivateKeyInAdRpc()
        {
            _keyInAd.SetActive(true);
        }

    }
    
}
