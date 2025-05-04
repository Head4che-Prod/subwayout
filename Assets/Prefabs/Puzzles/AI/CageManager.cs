using JetBrains.Annotations;
using Objects;
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
        
        void Start()
        {
            cheeseInCage.SetActive(false);
            animator = transform.GetChild(0).GetComponent<Animator>();
        }
        
        public void Action()
        {
            animator.SetBool(animCageDoor, !animator.GetBool(animCageDoor));
            cheeseGrabbable = PlayerInteract.LocalPlayerInteract.GrabbedObject;
            if (cheeseGrabbable !=null && cheeseGrabbable.name == "cheese(Clone)" && animator.GetBool(animCageDoor))
            {
                DeactivateCheese();
                cheeseInCage.SetActive(true);
            }
        }
        
        /// <summary>
        /// Drops the cheese and call DisableCheeseRpc.
        /// </summary>
        public void DeactivateCheese()
        {
            Drop();
            DisableCheeseRpc();
        }
        /// <summary>
        /// Removes the grabbableCheese.
        /// </summary>
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void DisableCheeseRpc()
        {
            cheeseGrabbable.NetworkObject.Despawn();
        }
        
    }
}
