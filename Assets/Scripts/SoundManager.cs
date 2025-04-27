using Unity.Netcode;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager singleton;
    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlaySoundRpc(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        
        
        audioSource.clip = audioClip;
        
        audioSource.volume = volume;
        
        audioSource.Play();
        
        float clipLength = audioSource.clip.length;
        
        Destroy(audioSource.gameObject, clipLength);
    }

}
