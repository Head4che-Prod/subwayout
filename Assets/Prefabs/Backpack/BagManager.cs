using System.Collections.Generic;
using Hints;
using Objects;
using Prefabs.GameManagers;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Backpack
{
    public class BagManager: ObjectGrabbable, IObjectInteractable
    {
        [Header("Bag")]
        [SerializeField] private GameObject bagOpen;
        [SerializeField] private GameObject bagClose;
        [SerializeField] private List<GameObject> objectsInBag;

        public void Action()
        {
            Debug.Log("Action played");
            ObjectHighlightManager.ForgetHighlightableObject(NetworkObjectId);
            OpenBagClientRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void OpenBagClientRpc()
        {
            bagOpen.SetActive(true);
            bagClose.SetActive(false);
            HintSystem.DisableHints(Hint.BackPack);
            canBeHighlighted = true;
            
            if (IsServer)
                ApplyObjectImpulses();
        }
        
        private void ApplyObjectImpulses()
        {
            foreach (GameObject obj in objectsInBag)
            {
                NetworkObject spawnedObj = Instantiate(obj, bagOpen.transform.position + new Vector3(0, 1, 0),
                    Quaternion.identity).GetComponent<NetworkObject>();
                spawnedObj.Spawn();
                ObjectHighlightManager.RegisterHighlightableObject(spawnedObj.NetworkObjectId);
                spawnedObj.GetComponent<Rigidbody>().AddForce(new Vector3(-1.5f, 2f, 0f), ForceMode.VelocityChange);
            }
        }
    }
}
