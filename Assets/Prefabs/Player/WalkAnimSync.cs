using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
	public class WalkAnimSync : MonoBehaviour
	{
		private Animator _playerAnimator;
		void Start()
		{
			int i = 1;
			while (!transform.GetChild(0).GetChild(i).gameObject.activeInHierarchy) i++;
			
			_playerAnimator = transform.GetChild(0).GetChild(i).GetComponent<Animator>();
		}

		public void CallRpc(bool setAnimation) => SendAnimRpc(setAnimation);
		
		[Rpc(SendTo.Everyone)]
		private void SendAnimRpc(bool setAnimation)
		{
			_playerAnimator.SetBool("isWalking", setAnimation);
		}
		
		
	}
}
