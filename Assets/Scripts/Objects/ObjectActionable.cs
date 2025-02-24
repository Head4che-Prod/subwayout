using JetBrains.Annotations;
using UnityEngine;

namespace Objects
{
    /// <summary>
    /// Allow players to action an object.
    /// </summary>
    public abstract class ObjectActionable : ObjectInteractive
    {
        [Header("animation")]
        [SerializeField] [CanBeNull] private NetworkAnimatorP2P animator;
        [SerializeField] [CanBeNull] private string animationName;

        /// <summary>
        /// This function handle the Action function with the animation.
        /// </summary>
        public void HandleAction()
        {
            if (animator is not null && animationName is not null)
                animator.Animator.SetBool(animationName, true);
            Action();
        }

        /// <summary>
        /// Allow to perform specifics actions depending on object actioned.
        /// </summary>
        public abstract void Action();
    }
}