using Prefabs.Player;

namespace Objects
{
    /// <summary>
    /// Allows players to action an object.
    /// </summary>
    public abstract class ObjectActionable : ObjectInteractable
    {
        
        /// <summary>
        /// This function handles the <see cref="Action"/> function with the animation.
        /// </summary>
        public void HandleAction()
        {
            Action();
        }

        /// <summary>
        /// Allows specific actions to be performed depending on the object actioned.
        /// <see cref="ObjectActionable"/>'s subclasses define the behavior of a specific (type of) object.
        /// </summary>
        protected abstract void Action();
    }
}