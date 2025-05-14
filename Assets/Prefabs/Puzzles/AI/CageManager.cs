using JetBrains.Annotations;
using Objects;
using Prefabs.GameManagers;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.AI
{ 
    public class CageManager : ObjectGrabbable, IObjectActionable
    {
        
        [CanBeNull] private ObjectGrabbable cheeseGrabbable;
        [SerializeField] private GameObject cheeseInCage; 
        private Animator animator;
        private static readonly int animCageDoor = Animator.StringToHash("animCageDoor");
        [SerializeField] private GameObject clonedRat;
        private readonly NetworkVariable<bool> _isOpen = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);

        
        void Start()
        {
            base.Start();
            cheeseInCage.SetActive(false);
            animator = transform.GetChild(0).GetComponent<Animator>();
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _isOpen.OnValueChanged += UpdatePosition;
        }
        
        public void Action()
        {
            if (clonedRat.activeSelf)
            {
                return;
            }
            
            cheeseGrabbable = (ObjectGrabbable)PlayerInteract.LocalPlayerInteract.GrabbedObject;
            if (cheeseGrabbable !=null && cheeseGrabbable.name == "cheese(Clone)" && animator.GetBool(animCageDoor))
            {
                DeactivateCheese();
            }
            else
            {
                ChangeCageDoorServerRpc(!animator.GetBool(animCageDoor));
            }
        }
        
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void ChangeCageDoorServerRpc(bool isOpenValChanged)
        {
            _isOpen.Value = isOpenValChanged;
        }
        /// <summary>
        /// Changes the door's position.
        /// </summary>
        private void UpdatePosition(bool _, bool newValue)
        {
            animator.SetBool(animCageDoor, newValue);
        }
        
        /// <summary>
        /// Drops the cheese and call DisableCheeseRpc.
        /// </summary>
        public void DeactivateCheese()
        {
            cheeseGrabbable!.Drop();
            DisableCheeseRpc(cheeseGrabbable.NetworkObjectId);
            ActivateCheeseInCageRpc();
        }
        /// <summary>
        /// Removes the grabbableCheese.
        /// </summary>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void DisableCheeseRpc(ulong cheeseId)
        {
            ObjectHighlightManager.ForgetHighlightableObject(cheeseId);
            NetworkManager.SpawnManager.SpawnedObjects[cheeseId].Despawn();
        }

        [Rpc(SendTo.Everyone)]
        private void ActivateCheeseInCageRpc()
        {
            cheeseInCage.SetActive(true);
        }
        
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _isOpen.OnValueChanged -= UpdatePosition;
        }
        
    }
}
