using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
	public class WalkAnimSync : NetworkBehaviour
	{
		private readonly int _isWalking = Animator.StringToHash("isWalking");
		private Animator _playerAnimator;
		public void Init()
		{
			int i = 1;
			while (!transform.GetChild(0).GetChild(i).gameObject.activeInHierarchy) i++;
			
			_playerAnimator = transform.GetChild(0).GetChild(i).GetComponent<Animator>();
		}

		public void CallWalkAnimationRpc(bool setAnimation) => SendAnimRpc(setAnimation);
		
		[Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
		private void SendAnimRpc(bool setAnimation)
		{
			_playerAnimator.SetBool(_isWalking, setAnimation);
		}
		
		
	}
}
