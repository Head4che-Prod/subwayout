using UnityEngine;

namespace Objects
{
    public interface IResettablePosition
    {
        public Vector3 InitialPosition { get; set; }
        public Quaternion InitialRotation { get; set; }

        public void ResetPosition();
    }
}