using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Random = System.Random;

namespace Hints
{
    public enum Hint
    {
        HintTutorial,
        NoHints,
        BlackboxLocation,
        SeatPuzzle,
        BlueCode,
        CombineSticker,
        SeatsSticker,
        RedCode,
        BackPack,
        RatTrap,
        RatKey,
        Numbers,
        Code,
        CodeUnlocks,
        Hanoi,
        GameWon
    }
    
    public class HintSystem
    {
        private static readonly Random Random = new Random(69);
        private static bool _firstInteractionPlayed;
        
        private static readonly List<Hint> Hints = new List<Hint>();
        public static Dictionary<Hint, HintSystem> HintIndex { get; } = new Dictionary<Hint, HintSystem>();

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

        public static Hint GetRandomVoiceLine()
        {
            if (!_firstInteractionPlayed)
            {
                _firstInteractionPlayed = true;
                return Hint.HintTutorial;
            }
            if (Hints.Count == 0)
                return Hint.NoHints;
            return Hints[Random.Next(Hints.Count)];
        }

        /// <summary>
        /// Adds the specified hints to the list of currently available hints.
        /// </summary>
        /// <param name="hints"><see cref="Hint"/>s to add.</param>
        public static void EnableHints(params Hint[] hints)
        {
            foreach (Hint hint in hints)
                if (!Hints.Contains(hint))
                    Hints.Add(hint);
        }

        /// <summary>
        /// Removes the specified hints to the list of currently available hints.
        /// </summary>
        /// <param name="hints"><see cref="Hint"/>s to remove.</param>
        public static void DisableHints(params Hint[] hints)
        {
            foreach (Hint hint in hints)
                if (Hints.Contains(hint))
                    Hints.Remove(hint);
        }

    }
}