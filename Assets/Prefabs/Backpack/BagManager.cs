using System.Collections.Generic;
using Hints;
using Objects;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Backpack
{
    public class BagManager: NetworkBehaviour, IObjectActionable
    {
        [SerializeField] private GameObject _bagOpen;
        [SerializeField] private GameObject _bagClose;
        [SerializeField] private List<GameObject> objectsInBag;

        public void Action()
        {
            Debug.Log("Action played");
            _bagOpen.SetActive(true);
            _bagClose.SetActive(false);
            ImpulsionObjetsRpc();
            HintSystem.DisableHints(Hint.BackPack);
        }
    
        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void ImpulsionObjetsRpc()
        {
            foreach (GameObject obj in objectsInBag)
            {
                GameObject spawnedObj = Instantiate(obj, _bagOpen.transform.position + new Vector3(0, 1, 0),
                    Quaternion.identity);
                spawnedObj.GetComponent<NetworkObject>().Spawn();
                spawnedObj.GetComponent<Rigidbody>().AddForce(new Vector3(-1.5f, 2f, 0f), ForceMode.VelocityChange);
            }
        }
    
    
    }
}
