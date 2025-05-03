using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Prefabs.Blackbox
{
    public class BlackBox : NetworkBehaviour
    {
        [SerializeField] private Animator slideAnimator;
        private bool _isPulledOut = false;

        public void Start()
        {
            slideAnimator = GetComponent<Animator>();
        }
        
        public void Action()
        {
            if (!_isPulledOut)
            {
                Debug.Log("Action");
                PullOutClientRpc();
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void PullOutClientRpc()
        {
            _isPulledOut = true; 
            // Only ever called once, no need to hash.
            slideAnimator.SetTrigger("slideBox");
        }
    }
}
