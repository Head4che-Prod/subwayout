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
    /// Allows players to action an object.
    /// </summary>
    public abstract class ObjectActionable : ObjectInteractable
    {
        protected abstract ActionableType ActionableType { get; }

        [Header("Animation")]
        [SerializeField] [CanBeNull] private NetworkAnimatorP2P animator;
        [SerializeField] [CanBeNull] private string animationName;
        
        /// <summary>
        /// This function handles the <see cref="Action"/> function with the animation.
        /// </summary>
        public void HandleAction()
        {
            if (animator is not null && animationName is not null)
                animator.Animator.SetBool(animationName, true);
            Action();
        }

        /// <summary>
        /// Allows specific actions to be performed depending on the object actioned.
        /// <see cref="ObjectActionable"/>'s subclasses define the behavior of a specific (type of) object.
        /// </summary>
        protected abstract void Action();
    }
}