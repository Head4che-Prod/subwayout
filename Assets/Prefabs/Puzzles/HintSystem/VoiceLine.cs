using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace Prefabs.Puzzles.HintSystem
{
    public abstract class VoiceLine
    {
        public string ID;
        public string PathEn;
        public string PathFr;
        public string PathEs;
        
        public static void LoadVoiceLines()
        {
            VoiceLine[] voiceLines = JsonConvert.DeserializeObject<VoiceLine[]>(
                File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "hints.json")));
            foreach (VoiceLine voiceLine in voiceLines)
            {
                PuzzleHint.HintIndex.Add(voiceLine.ID,
                    new PuzzleHint(
                        Resources.Load<AudioClip>("Audio/Hints/"+voiceLine.PathEn),
                        Resources.Load<AudioClip>("Audio/Hints/"+voiceLine.PathFr),
                        Resources.Load<AudioClip>("Audio/Hints/"+voiceLine.PathEs)
                        ));
            }
        }
    }
}