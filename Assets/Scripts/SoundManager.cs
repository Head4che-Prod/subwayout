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

    public void PlaySound(AudioClip audioClip, Transform spawnTansform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTansform.position, Quaternion.identity);
        
        audioSource.clip = audioClip;
        
        audioSource.volume = volume;
        
        audioSource.Play();
        
        float clipLength = audioSource.clip.length;
        
        Destroy(audioSource.gameObject, clipLength);
    }

}
