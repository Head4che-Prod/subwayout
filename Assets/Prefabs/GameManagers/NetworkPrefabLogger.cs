using System.Collections.Generic;
using Prefabs.Player.PlayerUI.DebugConsole;
using Unity.Netcode;
using UnityEditor;
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

#if UNITY_EDITOR
        /// <summary>
        /// Checks for unregistered Network Prefabs.
        /// DISCLAIMER: Head4che did not write this code.
        /// </summary>
        [ContextMenu("Check for Unregistered NetworkObjects")]
        public void CheckUnregisteredPrefabs()
        {
            HashSet<ulong> registered = new HashSet<ulong>();

            foreach (NetworkPrefab prefab in NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs)
            {
                if (prefab.Prefab != null && prefab.Prefab.TryGetComponent(out NetworkObject netObj))
                {
                    registered.Add(netObj.PrefabIdHash);
                }
            }

            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                    continue;

                // Check for missing script components
                var components = prefab.GetComponents<Component>();
                bool hasMissing = false;
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        Debug.LogWarning($"❌ Missing script on prefab: {prefab.name} at index {i}, path: {path}");
                        hasMissing = true;
                    }
                }

                // Check if it's a NetworkObject that's unregistered
                if (prefab.TryGetComponent(out NetworkObject no))
                {
                    if (!registered.Contains(no.PrefabIdHash))
                    {
                        Debug.LogWarning(
                            $"⚠️ Unregistered NetworkObject prefab: {prefab.name}, PrefabIdHash: {no.PrefabIdHash}, Path: {path}");
                    }
                }

                // Optional: flag completely invalid prefabs with missing and unregistered
                if (hasMissing)
                {
                    Debug.Log($"ℹ️ Skipped prefab with missing scripts: {prefab.name}");
                }
            }
        }
#endif
    }
}