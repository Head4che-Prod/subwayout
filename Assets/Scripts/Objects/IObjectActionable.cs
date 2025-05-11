using Prefabs.Player;

namespace Objects
{
    /// <summary>
    /// Allows players to action an object.
    /// </summary>
    public interface IObjectActionable : IRaycastResponsive
    {
        /// <summary>
        /// Allows specific actions to be performed depending on the object actioned.
        /// <see cref="IObjectActionable"/>'s subclasses define the behavior of a specific (type of) object.
        /// </summary>
        public void Action();
    }
}