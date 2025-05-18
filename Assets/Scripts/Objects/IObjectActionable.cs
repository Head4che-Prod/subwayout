using JetBrains.Annotations;
using Prefabs.GameManagers;
using Prefabs.Player;
using UnityEngine;

namespace Objects
{
    /// <summary>
    /// Allows players to action an object.
    /// </summary>
    public interface IObjectActionable : IRaycastResponsive
    {
        [CanBeNull] public string soundEffectName => null;

        /// <summary>
        /// Allows specific actions to be performed depending on the object actioned.
        /// <see cref="IObjectActionable"/>'s subclasses define the behavior of a specific (type of) object.
        /// </summary>
        public void Action();
        public void HandleAction()
        {
            Action();
            
            if (soundEffectName is not null and not "")
                SoundManager.Singleton.PlaySoundRpc(soundEffectName, this is MonoBehaviour mb? mb.transform.position: new Vector3(0, 0, 0), 1f); // Modified to be an RPC
        }
    }
}