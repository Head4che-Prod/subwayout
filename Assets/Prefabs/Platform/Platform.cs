using System.Collections;
using Prefabs.GameManagers;
using Prefabs.Player;
using UnityEngine;

namespace Prefabs.Platform
{
    public class Platform : MonoBehaviour
    {
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
    }
}
