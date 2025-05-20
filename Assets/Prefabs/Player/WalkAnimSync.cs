using System;
using Unity.Netcode;
using UnityEngine;


namespace Prefabs.Player
{
	public class WalkAnimSync : NetworkBehaviour
	{
		private int _isWalking;
		private Animator _playerAnimator;
		
		/// <summary>
		/// The method to call when the animation should be synced. Is the null action until the system is initialized.
		/// </summary>
		private Action<bool> _walkAnimationDelegate = _ => { };
		
		/// <summary>
		/// Initializes the walk animation script. Called once a player's skin is registered.
		/// </summary>
		public void Init()
		{
			int i = 1;
			Transform character = transform.GetChild(0);
			while (!character.GetChild(i).gameObject.activeInHierarchy) i++;
			
			_playerAnimator = transform.GetChild(0).GetChild(i).GetComponent<Animator>();
			Debug.Log($"isWalking{_playerAnimator.gameObject.name}");
			_isWalking = Animator.StringToHash($"isWalking{_playerAnimator.gameObject.name}");
			_walkAnimationDelegate = SendAnimRpc;
		}

		public void CallWalkAnimationRpc(bool setAnimation) => _walkAnimationDelegate(setAnimation);
		
		[Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
		private void SendAnimRpc(bool setAnimation)
		{
			_playerAnimator.SetBool(_isWalking, setAnimation);
		}
		
		
	}
}
