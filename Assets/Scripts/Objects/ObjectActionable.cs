using Prefabs.Player;
using UnityEngine;
namespace Objects
{
    /// <summary>
    /// Allows players to action an object.
    /// </summary>
    public abstract class ObjectActionable : ObjectInteractable
    {
        [SerializeField] AudioClip soundEffect;
        
        /// <summary>
        /// This function handles the <see cref="Action"/> function with the animation.
        /// </summary>
        public void HandleAction(PlayerObject player)
        {
            Action(player);
            
            if (soundEffect != null)
                SoundManager.singleton.PlaySoundRpc(soundEffect, transform, 1f); // Modified to be an RPC
        }

        /// <summary>
        /// Allows specific actions to be performed depending on the object actioned.
        /// <see cref="ObjectActionable"/>'s subclasses define the behavior of a specific (type of) object.
        /// </summary>
        protected abstract void Action(PlayerObject player);
    }
}