using System;
using Objects;
using Prefabs.Player;
using UnityEngine;

namespace Prefabs.Puzzles.AI
{ 
    public class CageManager : ObjectGrabbable, IObjectActionable
    {
        
        private ObjectGrabbable cheeseGrabbable;
        [SerializeField] private GameObject cheeseInCage; 
        private Animator animator;
        private static readonly int animCageDoor = Animator.StringToHash("animCageDoor");
        
        void Start()
        {
            cheeseInCage.SetActive(false);
            animator = GetComponent<Animator>();
        }
        
        public void Action()
        {
            animator.SetBool(animCageDoor, !animator.GetBool(animCageDoor));
            cheeseGrabbable = PlayerInteract.LocalPlayerInteract.GrabbedObject;
            if ( cheeseGrabbable.name == "cheese(Clone)" && animator.GetBool(animCageDoor))
            {
                cheeseGrabbable.gameObject.SetActive(false);
                cheeseInCage.SetActive(true);
            }
            
        }
    }
}
