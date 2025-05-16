using Objects;
using Prefabs.GameManagers;
using Prefabs.Player;
using Prefabs.Puzzles.AI.Key;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.AI.CheeseAd
{
    public class TomeTomeAdManager : NetworkBehaviour,IObjectActionable
    {
        [SerializeField] private GameObject keyInAd;
        private Animator _animator;
        private ObjectGrabbable _grabbedObject;
        private static readonly int OpenCheeseAdAnim = Animator.StringToHash("openCheeseAd");
        private readonly NetworkVariable<bool> _isOpen = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
        

        void Start()
        {
            keyInAd.SetActive(false);
            _animator = transform.GetChild(0).GetComponent<Animator>();
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _isOpen.OnValueChanged += UpdatePosition;
        }
        

        public void Action()
        {
            _grabbedObject = PlayerInteract.LocalPlayerInteract.GrabbedObject?.GrabbedObject;
            if (_grabbedObject is KeyGrabbable)
            {
                DeactivateGrabbedKey();
            }
            else if (keyInAd.activeInHierarchy)
            {
                ChangeAdDoorServerRpc(!_animator.GetBool(OpenCheeseAdAnim));
            }
        }
        
        /// <summary>
        /// Requests the server to change the internal state of the ad case's door.
        /// </summary>
        /// <param name="isOpenValChanged">Whether the door must be opened.</param>
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
            _animator.SetBool(OpenCheeseAdAnim, newValue);
        }
        
        /// <summary>
        /// Deactivate the grabbed key but activate the 3D model in the ad.
        /// </summary>
        private void DeactivateGrabbedKey()
        {
            _grabbedObject!.Drop();
            DisableKeyGrabbableRpc(_grabbedObject.NetworkObjectId);
            ActivateKeyInAdRpc();
        }
        /// <summary>
        /// Removes the key that was grabbed.
        /// </summary>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void DisableKeyGrabbableRpc(ulong keyGrabbedID)
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
            keyInAd.SetActive(true);
        }

    }
    
}
