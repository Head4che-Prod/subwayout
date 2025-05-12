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
        
        void Start()
        {
            base.Start();
            cheeseInCage.SetActive(false);
            animator = transform.GetChild(0).GetComponent<Animator>();
        }
        
        public void Action()
        {
            if (clonedRat.activeSelf)
            {
                animator.SetBool(animCageDoor, !animator.GetBool(animCageDoor));
                return;
            }
            
            cheeseGrabbable = PlayerInteract.LocalPlayerInteract.GrabbedObject;
            if (cheeseGrabbable !=null && cheeseGrabbable.name == "cheese(Clone)" && animator.GetBool(animCageDoor))
            {
                DeactivateCheese();
                cheeseInCage.SetActive(true);
            }
            else
            {
                animator.SetBool(animCageDoor, !animator.GetBool(animCageDoor));
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
            ObjectHighlightManager.ForgetHighlightableObject(cheeseGrabbable!.NetworkObjectId);
            cheeseGrabbable!.NetworkObject.Despawn();
        }
        
    }
}
