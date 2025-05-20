using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
	public class WalkAnimSync : NetworkBehaviour
	{
		private int _isWalking;
		private Animator _playerAnimator;
		public void Init()
		{
			int i = 1;
			while (!transform.GetChild(0).GetChild(i).gameObject.activeInHierarchy) i++;
			
			_playerAnimator = transform.GetChild(0).GetChild(i).GetComponent<Animator>();
			Debug.Log($"isWalking{_playerAnimator.gameObject.name}");
			_isWalking = Animator.StringToHash($"isWalking{_playerAnimator.gameObject.name}");
		}

		public void CallWalkAnimationRpc(bool setAnimation) => SendAnimRpc(setAnimation);
		
		[Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
		private void SendAnimRpc(bool setAnimation)
		{
			Debug.Log($"Moving: isWalking{_playerAnimator.gameObject.name}");
			_playerAnimator.SetBool(_isWalking, setAnimation);
		}
		
		
	}
}
