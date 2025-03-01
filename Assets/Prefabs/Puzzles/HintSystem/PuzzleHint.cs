using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Random = System.Random;

namespace Prefabs.Puzzles.HintSystem
{
    public class PuzzleHint
    {
        private static Random _r = new Random(69);
        
        public static List<string> Hints { get; } = new List<string>();
        public static Dictionary<string, PuzzleHint> HintIndex { get; } = new Dictionary<string, PuzzleHint>();

        private readonly AudioClip _voiceLineEn;
        private readonly AudioClip _voiceLineFr;
        private readonly AudioClip _voiceLineEs;

        private AudioClip _voiceLine
        {
            get
            {
                switch (LocalizationSettings.SelectedLocale.Identifier.Code)
                {
                    case "en-US":
                        return _voiceLineEn;
                    case "fr-FR":
                        return _voiceLineFr;
                    case "es-ES":
                        return _voiceLineEs;
                    default:
                        Debug.LogError($"Language \"{LocalizationSettings.SelectedLocale.Identifier.Code}\" not found");
                        return _voiceLineEn;
                }
            }
        }

        public PuzzleHint(AudioClip voiceLineEn, AudioClip voiceLineFr, AudioClip voiceLineEs)
        {
            _voiceLineEn = voiceLineEn;
            _voiceLineFr = voiceLineFr;
            _voiceLineEs = voiceLineEs;
        }

        public static AudioClip GetRandomVoiceLine()
        {
            if (Hints.Count == 0)
                return HintIndex["NoHints"]._voiceLine;
            return HintIndex[Hints[_r.Next(Hints.Count)]]._voiceLine;
        }

    }
}