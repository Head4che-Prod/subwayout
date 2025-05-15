using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Random = System.Random;

namespace Prefabs.Puzzles.HintSystem
{
    public class PuzzleHint
    {
        private static Random _r = new Random(69);
        private static bool _firstInteractionPlayed;
        
        public static List<string> Hints { get; } = new List<string>();
        public static Dictionary<string, PuzzleHint> HintIndex { get; } = new Dictionary<string, PuzzleHint>();

        private readonly AudioClip _voiceLineEn;
        private readonly AudioClip _voiceLineFr;
        private readonly AudioClip _voiceLineEs;
        private readonly float _durationEn;
        private readonly float _durationFr;
        private readonly float _durationEs;

        public AudioClip VoiceLine
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

        public float Duration
        {
            get
            {
                switch (LocalizationSettings.SelectedLocale.Identifier.Code)
                {
                    case "en-US":
                        return _durationEn;
                    case "fr-FR":
                        return _durationFr;
                    case "es-ES":
                        return _durationEs;
                    default:
                        Debug.LogError($"Language \"{LocalizationSettings.SelectedLocale.Identifier.Code}\" not found");
                        return _durationEn;
                }
            }
        }
        
        public PuzzleHint(AudioClip voiceLineEn, AudioClip voiceLineFr, AudioClip voiceLineEs)
        {
            _voiceLineEn = voiceLineEn;
            _voiceLineFr = voiceLineFr;
            _voiceLineEs = voiceLineEs;
            _durationEn = _voiceLineEn.length;
            _durationFr = _voiceLineFr.length;
            _durationEs = _voiceLineEs.length;
        }

        public static string GetRandomVoiceLine()
        {
            if (!_firstInteractionPlayed)
            {
                _firstInteractionPlayed = true;
                return "HintTutorial";
            }
            if (Hints.Count == 0)
                return "NoHints";
            return Hints[_r.Next(Hints.Count)];
        }

    }
}