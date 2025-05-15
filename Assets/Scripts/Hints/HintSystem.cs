using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Random = System.Random;

namespace Hints
{
    public class HintSystem
    {
        private static readonly Random andom = new Random(69);
        private static bool _firstInteractionPlayed;
        
        private static readonly List<string> Hints = new List<string>();
        public static Dictionary<string, HintSystem> HintIndex { get; } = new Dictionary<string, HintSystem>();

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
        
        public HintSystem(AudioClip voiceLineEn, AudioClip voiceLineFr, AudioClip voiceLineEs)
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
            return Hints[andom.Next(Hints.Count)];
        }

        public static void EnableHints(params string[] hints)
        {
            foreach (string hint in hints)
                if (!Hints.Contains(hint))
                    Hints.Add(hint);
        }

        public static void DisableHints(params string[] hints)
        {
            foreach (string hint in hints)
                if (Hints.Contains(hint))
                    Hints.Remove(hint);
        }

    }
}