using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Hints
{
    public class VoiceLine
    {
        public string ID;
        public string PathEn;
        public string PathFr;
        public string PathEs;

        public static void LoadVoiceLines()
        {
            AudioClip fallback = Resources.Load<AudioClip>("Audio/Hints/Placeholders/neige_fixed");
                
            VoiceLine[] voiceLines = JsonConvert.DeserializeObject<VoiceLine[]>(
                File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "hints.json")));
            foreach (VoiceLine voiceLine in voiceLines)
            {
                if (Enum.TryParse<Hint>(voiceLine.ID, out Hint hint))
                    HintSystem.HintIndex.Add(hint,
                        new HintSystem(
                            Resources.Load<AudioClip>("Audio/Hints/"+voiceLine.PathEn) ?? fallback,
                            Resources.Load<AudioClip>("Audio/Hints/"+voiceLine.PathFr) ?? fallback,
                            Resources.Load<AudioClip>("Audio/Hints/"+voiceLine.PathEs) ?? fallback
                            ));
                else 
                    Debug.LogError($"'{voiceLine.ID}' is not a valid hint id.");
            }
        }
    }
}