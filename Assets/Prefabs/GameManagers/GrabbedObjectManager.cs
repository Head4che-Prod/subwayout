using System.Collections.Generic;
using System.Linq;
using Objects;
using Prefabs.Player;
using Prefabs.Player.PlayerUI.DebugConsole;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.GameManagers
{
    public class GrabbedObjectManager : NetworkBehaviour
    {
        private readonly Dictionary<PlayerObject, ObjectGrabbable> _grabbedObjects =
            new Dictionary<PlayerObject, ObjectGrabbable>();

        private static GrabbedObjectManager _instance;

        private static GrabbedObjectManager Instance
        {
            get
            {
                if (!_instance)
                    Debug.LogError("No GrabbedObjectManager found.");
                else if (!_instance.IsServer)
                    Debug.LogWarning(
                        $"Non-server client {NetworkManager.Singleton.LocalClientId} is trying to access grabbed object manager.");
                return _instance;
            }
            set
            {
                if (!_instance)
                    _instance = value;
                else
                    Debug.LogError("Attempting to create a new GrabbedObjectManager.");
            }
        }

        private static PlayerObject GetPlayer(ulong clientId) => NetworkManager.Singleton.ConnectedClients
            .First(pair => pair.Key == clientId).Value.PlayerObject.GetComponent<PlayerObject>();

        private static NetworkObject GetObject(ulong objectNetworkId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectNetworkId,
                    out NetworkObject obj))
                return obj;
            Debug.LogError($"Could not find NetworkObject of hash {objectNetworkId}.");
            return null;
        }

        public void Start()
        {
            DebugConsole.AddCommand("listCurrentlyHolding", ListCurrentlyHolding);
            if (IsHost)
            {
                Instance = this;
                Instance._grabbedObjects.Clear();
                foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                    Instance._grabbedObjects.Add(client.PlayerObject.GetComponent<PlayerObject>(), null);
            }
        }
        
        public static void PlayerGrab(ulong clientId, ulong objectNetworkId)
        {
            Instance._grabbedObjects[GetPlayer(clientId)] = GetObject(objectNetworkId).GetComponent<ObjectGrabbable>();
        }

        public static void ForgetPlayer(PlayerObject player)
        {
            ObjectGrabbable obj = Instance._grabbedObjects[player];
            if (obj != null) obj.Grabbable = true;
            Instance._grabbedObjects.Remove(player);
        }

        public static void PlayerDrop(ulong clientId)
        {
            Instance._grabbedObjects[GetPlayer(clientId)] = null;
        }

        public static void ForgetObject(ObjectGrabbable obj)
        {
            foreach (KeyValuePair<PlayerObject, ObjectGrabbable> pair in Instance._grabbedObjects)
                if (pair.Value == obj)
                {
                    Instance._grabbedObjects[pair.Key] = null;
                    break;
                }
        }

        private void FixedUpdate()
        {
            if (!IsServer)
                return;

            foreach (PlayerObject player in
                     _grabbedObjects.Keys.ToArray()) // ToArray is order to not modify collection while iterating
            {
                ObjectGrabbable obj = _grabbedObjects[player];
                if (obj)
                    obj.Rb.linearVelocity = obj.CalculateMovementForce(player) * obj.moveSpeed;
            }

        }

        private static void ListCurrentlyHolding()
        {
            if (!NetworkManager.Singleton.IsHost)
                DebugConsole.Singleton.Log("Only the host know which objects players are holding.");
            else
                foreach (KeyValuePair<PlayerObject, ObjectGrabbable> pair in Instance._grabbedObjects)
                    DebugConsole.Singleton.Log(
                        $"Player {pair.Key.GetComponent<NetworkObject>().OwnerClientId} - {pair.Value?.gameObject.name ?? "nothing"}");
        }
    }
}