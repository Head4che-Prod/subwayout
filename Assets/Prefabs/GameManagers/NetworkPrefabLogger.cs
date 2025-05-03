using Prefabs.Player.PlayerUI.DebugConsole;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.GameManagers
{
    public class NetworkPrefabLogger : MonoBehaviour
    {
        public void Start()
        {
           DebugConsole.AddCommand("listNetworkPrefabIds", ListNetworkPrefabIds);
        }

        /// <summary>
        /// Lists the hashes of all spawned NetworkPrefabs. Used to find which object causes an error over the Network.
        /// </summary>
        private void ListNetworkPrefabIds()
        {
            foreach (NetworkPrefab prefab in NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs)
            {
                if (prefab.Prefab.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
                    DebugConsole.Singleton.Log($"Id {networkObject.PrefabIdHash} - {prefab.Prefab.name}");
            }
        }
    }
}
