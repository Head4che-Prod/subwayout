using JetBrains.Annotations;
using Prefabs.Player;
using UnityEngine;

namespace Objects
{
    /// <summary>
    /// Allows players to action an object.
    /// </summary>
    public abstract class ObjectActionable : ObjectInteractable
    {
        [SerializeField] [CanBeNull] private Animator objectAnimator;
        
        /// <summary>
        /// This function handles the <see cref="Action"/> function with the animation.
        /// </summary>
        public void HandleAction(PlayerObject player)
        {
            if (objectAnimator != null) Animate(objectAnimator);
            Action(player);
        }

        /// <summary>
        /// Allows specific actions to be performed depending on the object actioned.
        /// <see cref="ObjectActionable"/>'s subclasses define the behavior of a specific (type of) object.
        /// </summary>
        protected abstract void Action(PlayerObject player);
        protected abstract void Animate(Animator objectAnimator);
    }
}