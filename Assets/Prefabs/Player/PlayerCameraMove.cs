using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Player
{
    public class MoveCamera : NetworkBehaviour
    {
        public GameObject playerCamera;
        public Transform cameraPosition;

        private void Start()
        {
            playerCamera.gameObject.SetActive(IsLocalPlayer);
        }

        private void Update()
        {
            transform.position = cameraPosition.position;
        }
    }
}
