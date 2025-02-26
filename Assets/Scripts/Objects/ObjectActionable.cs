using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Objects
{
    /// <summary>
    /// Enum of <see cref="ObjectActionable"/>'s subclass
    /// </summary>
    public enum ActionableType
    {
        EmergencyTrigger,
        Backpack,
        MetroDoors,
        Trapdoor,
        AdvertisingDisplay
    }
    
    /// <summary>
    /// Allow players to action an object.
    /// </summary>
    public abstract class ObjectActionable : ObjectInteractable
    {
        public abstract ActionableType ActionableType { get; }

        [Header("Animation")]
        [SerializeField] [CanBeNull] private NetworkAnimatorP2P animator;
        [SerializeField] [CanBeNull] private string animationName;
        
        /// <summary>
        /// This function handle the <see cref="Action"/> function with the animation.
        /// </summary>
        public void HandleAction()
        {
            if (animator is not null && animationName is not null)
                animator.Animator.SetBool(animationName, true);
            Action();
        }

        /// <summary>
        /// Allow to perform specifics actions depending on object actioned.
        /// <see cref="ObjectActionable"/>'s subclasses are the behavior of a specific (type of) object.
        /// </summary>
        /// <remarks></remarks>
        public abstract void Action();
    }
}