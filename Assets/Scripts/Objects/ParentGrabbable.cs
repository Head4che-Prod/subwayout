using UnityEngine;

namespace Objects
{
    /// <summary>
    /// Indicates the object should grab another object when attempted to grab.
    /// </summary>
    public class ParentGrabbable : MonoBehaviour, IObjectGrabbable
    {
        [SerializeField] private ObjectGrabbable parent;

        public bool Grabbable
        {
            get => parent.Grabbable;
            set => parent.Grabbable = value;
        }
        
        public void Grab() => parent.Grab();
        
        public void Drop() => parent.Drop();
    }
}
