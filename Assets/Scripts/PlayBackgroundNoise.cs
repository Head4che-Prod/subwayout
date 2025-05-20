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
            yield return new WaitForSeconds(Random.Range(20, 30));
            if ((TutorialManager.Instance.State != TutorialState.TrainStopped) && EndGameManager.Instance.State == EndGameState.WaitingHanoi)
                SoundManager.Singleton.PlaySoundRpc("BackgroundNoise", transform.position);
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
