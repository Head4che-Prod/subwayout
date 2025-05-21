using JetBrains.Annotations;
using Objects;
using Prefabs.GameManagers;
using Prefabs.Player;
using Prefabs.Puzzles.AI.Cheese;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Prefabs.Puzzles.AI
{ 
    public class CageManager : ObjectGrabbable, IObjectInteractable
    {
        
        private static CageManager _singleton;

        public static CageManager Singleton
        {
            get
            {
                if (_singleton != null)
                    return _singleton;
                Debug.LogError("Cage singleton no set");
                return null;
            }
            private set
            {
                if (_singleton == null)
                    _singleton = value;
                else
                    Debug.LogError("Cage singleton already set!");
            }
        }
        
        
        
        
        [SerializeField] private GameObject cheeseInCage; 
        [SerializeField] private GameObject clonedRat;
        private Animator _animator;
        private ObjectGrabbable _grabbedObject;
        private static readonly int AnimCageDoor = Animator.StringToHash("animCageDoor");
        private readonly NetworkVariable<bool> _isOpen = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
        [SerializeField] public Collider _triggerCollider;
        
        new void Start()
        {
            base.Start();
            cheeseInCage.SetActive(false);
            _animator = transform.GetChild(0).GetComponent<Animator>();
            Singleton = this;
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

        void OnTriggerStay(Collider other)
        {
            _grabbedObject = PlayerInteract.LocalPlayerInteract.GrabbedObject?.GrabbedObject;
            if ( _grabbedObject is CheeseGrabbable cheeseGrabbable)
            {
                Collider coll =cheeseGrabbable.gameObject.GetComponent<Collider>();
                Debug.Log("cheese in contact with collider of the cage");
                if (_triggerCollider.bounds.Contains(coll.bounds.max) && _triggerCollider.bounds.Contains(coll.bounds.min))
                {
                    DeactivateCheese();
                }
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
            ObjectHighlightManager.ForgetHighlightableObject(_grabbedObject.NetworkObjectId);
            _grabbedObject!.Drop();
            DisableCheeseRpc(_grabbedObject.NetworkObjectId);
            ActivateCheeseInCageRpc();
            ObjectHighlightManager.ForgetHighlightableObject(NetworkObjectId);
        }
        
        /// <summary>
        /// Despawns the grabbableCheese.
        /// </summary>
        /// <param name="cheeseId">The cheese's NetworkObjectId.</param>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void DisableCheeseRpc(ulong cheeseId)
        {
            NetworkManager.SpawnManager.SpawnedObjects[cheeseId].Despawn();
        }

        /// <summary>
        /// Activates the cheese in the cage and disables highlight for both it and the cage.
        /// </summary>
        [Rpc(SendTo.ClientsAndHost)]
        private void ActivateCheeseInCageRpc()
        {
            cheeseInCage.SetActive(true);
            canBeHighlighted = false;
        }
        
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _isOpen.OnValueChanged -= UpdatePosition;
        }
        
    }
}
