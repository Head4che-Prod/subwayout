using JetBrains.Annotations;
using Objects;
using Prefabs.GameManagers;
using Prefabs.Player;
using Prefabs.Puzzles.AI.Cheese;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.AI
{ 
    public class CageManager : ObjectGrabbable, IObjectActionable
    {
        
        [SerializeField] private GameObject cheeseInCage; 
        [SerializeField] private GameObject clonedRat;
        private Animator _animator;
        private ObjectGrabbable _grabbedObject;
        private static readonly int AnimCageDoor = Animator.StringToHash("animCageDoor");
        private readonly NetworkVariable<bool> _isOpen = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);

        
        new void Start()
        {
            base.Start();
            cheeseInCage.SetActive(false);
            _animator = transform.GetChild(0).GetComponent<Animator>();
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

            _grabbedObject = PlayerInteract.LocalPlayerInteract.GrabbedObject?.GrabbedObject;
            if (_isOpen.Value && _grabbedObject is CheeseGrabbable)
            {
                DeactivateCheese();
            }
            else
            {
                ChangeCageDoorServerRpc(!_animator.GetBool(AnimCageDoor));
            }
        }
        
        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void ChangeCageDoorServerRpc(bool isOpenValChanged)
        {
            _isOpen.Value = isOpenValChanged;
        }
        /// <summary>
        /// Changes the door's position.
        /// </summary>
        private void UpdatePosition(bool _, bool newValue)
        {
            _animator.SetBool(AnimCageDoor, newValue);
        }
        
        
        /// <summary>
        /// Drops the cheese and call DisableCheeseRpc.
        /// </summary>
        private void DeactivateCheese()
        {
            _grabbedObject!.Drop();
            DisableCheeseRpc(_grabbedObject.NetworkObjectId);
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
