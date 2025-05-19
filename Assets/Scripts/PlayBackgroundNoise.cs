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
            yield return new WaitForSeconds(5f);
            SoundManager.Singleton.PlaySoundRpc("BackgroundNoise", transform.position, 0.5f);
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
