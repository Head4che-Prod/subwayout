using System;
using System.Collections;
using System.Collections.Generic;
using Prefabs.GameManagers;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayBackgroundNoise : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Timer());
    }
    

    IEnumerator Timer()
    { 
        while(true)
        {
            if ((TutorialManager.Instance.State != TutorialState.TrainStopped || !TutorialManager.CanBeChanged) && EndGameManager.Instance.State == EndGameState.WaitingHanoi)
                SoundManager.Singleton.PlaySoundRpc("BackgroundNoise", transform.position);
            yield return new WaitForSeconds(9);
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
