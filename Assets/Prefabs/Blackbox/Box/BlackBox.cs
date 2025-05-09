using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Blackbox.Box
{
    public class BlackBox : NetworkBehaviour
    {
        private Animator _slideAnimator;
        public bool IsPulledOut { get; private set; } = false;

        public void Start()
        {
            _slideAnimator = GetComponent<Animator>();
        }

        public void Action()
        {
            if (!IsPulledOut)
            {
                Debug.Log("Action");
                PullOutClientRpc();
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void PullOutClientRpc()
        {
            IsPulledOut = true;
            // Only ever called once, no need to hash.
            _slideAnimator.SetTrigger("slideBox");
        }
    }
}