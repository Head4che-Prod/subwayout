using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Prefabs.Player.PlayerUI.DebugConsole;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Prefabs.GameManagers
{
    public class SoundManager : NetworkBehaviour
    {
        private class JSONSoundData
        {
            public string ID { get; set; }
            public string path { get; set; }
        }
        public static SoundManager Singleton;
        [SerializeField] private AudioSource soundFXObject;
        private Dictionary<string, AudioClip> _sounds;

        private void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }

            _sounds = JsonConvert.DeserializeObject<JSONSoundData[]>(
                    File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "sounds.json")))
                .ToDictionary(t => t.ID, t => Resources.Load<AudioClip>("Audio/" + t.path));
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        public void PlaySoundRpc(string clipName, Vector3 position, float volume)
        {
            DebugConsole.Singleton.Log("Here");
            AudioSource audioSource = Instantiate(soundFXObject, position, Quaternion.identity);


            audioSource.clip = _sounds[clipName];

            audioSource.volume = volume;

            audioSource.Play();

            float clipLength = audioSource.clip.length;

            Destroy(audioSource.gameObject, clipLength);
        }
        
    }
}