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
                Hints.HintSystem.HintIndex.Add(voiceLine.ID,
                    new Hints.HintSystem(
                        Resources.Load<AudioClip>("Audio/Hints/"+voiceLine.PathEn) ?? fallback,
                        Resources.Load<AudioClip>("Audio/Hints/"+voiceLine.PathFr) ?? fallback,
                        Resources.Load<AudioClip>("Audio/Hints/"+voiceLine.PathEs) ?? fallback
                        ));
            }
        }
    }
}