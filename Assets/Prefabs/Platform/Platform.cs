using System.Collections;
using Prefabs.GameManagers;
using Prefabs.Player;
using UnityEngine;

namespace Prefabs.Platform
{
    public class Platform : MonoBehaviour
    {
        private Collider _collider;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
            EndGameManager.Instance.EndGameState.OnValueChanged += EnableCollision;
        }

        private void EnableCollision(EndGameState _, EndGameState newState)
        {
            if (newState == EndGameState.HanoiResolved)
                _collider.enabled = true;
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<PlayerObject>() == null || EndGameManager.Instance?.State != EndGameState.UnlockDoors)
                return;
            StartCoroutine(FinishGame());
        }

        private IEnumerator FinishGame()
        {
            yield return new WaitForSeconds(1);
            EndGameManager.Instance.State = EndGameState.FinishGame;
        }

        private void OnDestroy()
        {
            EndGameManager.Instance.EndGameState.OnValueChanged -= EnableCollision;
        }
    }
}
