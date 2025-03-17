using System.Collections.Generic;
using Prefabs.Player.PlayerUI.DebugConsole;
using Unity.Netcode;
using UnityEngine;

namespace Objects
{
    public class ObjectPositionManager : MonoBehaviour
    {
        private static ObjectPositionManager _singleton;
        
        public static ObjectPositionManager Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    Debug.LogError("Object position manager singleton has not been initialized.");
                }
                return _singleton;
            }
            private set
            {
                if (_singleton == null)
                {
                    _singleton = value;
                    DebugConsole.AddCommand("resetObjects", ResetPositionsServerRpc);
                }
                else
                {
                    Debug.LogError("Object position manager singleton already initialized.");
                }
            }
        }

        public void Start()
        {
            Singleton = this;
        }
        
        public List<IResettablePosition> ResettableObjects { get; } = new List<IResettablePosition>();

        [ServerRpc(RequireOwnership = false)]
        private static void ResetPositionsServerRpc()
        {
            ResetPositionsClientRpc();
        }

        [ClientRpc]
        private static void ResetPositionsClientRpc()
        {
            foreach (IResettablePosition resettableObject in _singleton.ResettableObjects)
                resettableObject.ResetPosition();
        }
    }
}