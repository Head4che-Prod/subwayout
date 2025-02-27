using Unity.Netcode;
using UnityEngine;

namespace Objects
{
    /// <summary>
    /// Allow players to place grabbed <see cref="ObjectGrabbable"/>.
    /// </summary>
    public class ObjectPlaceholder : ObjectInteractable
    {
        [SerializeField] private NetworkVariable<bool> isFree = new(true);

        public bool Free { get; set; }
    }
}