using System.Collections.Generic;
using Objects;
using Prefabs.Player.PlayerUI.DebugConsole;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.GameManagers
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
                    Debug.LogWarning("Object position manager singleton has not been initialized.");
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

        public void Awake()
        {
            Singleton = this;
        }
        
        public HashSet<IResettablePosition> ResettableObjects { get; } = new HashSet<IResettablePosition>();
        
        [Rpc(SendTo.Server,RequireOwnership = false)]
        private static void ResetPositionsServerRpc()
        {
            ResetPositionsClientRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private static void ResetPositionsClientRpc()
        {
            foreach (IResettablePosition resettableObject in _singleton.ResettableObjects)
                resettableObject.ResetPosition();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public static void ForgetResettableObjectClientRpc(IResettablePosition resettableObject)
        {
            _singleton.ResettableObjects.Remove(resettableObject);
        }
    }
}