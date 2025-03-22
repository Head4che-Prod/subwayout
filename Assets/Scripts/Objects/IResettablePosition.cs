using Prefabs.GameManagers;
using UnityEngine;

namespace Objects
{
    public interface IResettablePosition
    {
        public Vector3 InitialPosition { get; set; }
        public Quaternion InitialRotation { get; set; }

        public void RegisterInitialState(Vector3 position, Quaternion rotation)
        {
            InitialPosition = position;
            InitialRotation = rotation;
            ObjectPositionManager.Singleton.ResettableObjects.Add(this);
        }

        public void ResetPosition();
    }
}