using System.Collections.Generic;
using UnityEngine;

namespace Prefabs.Puzzles.HintSystem
{
    public class PuzzleHint
    {
        static List<string> Hints;
        static Dictionary<string, PuzzleHint> HintIndex { get; }
        private static readonly System.Random _r = new System.Random(69);
        
        private string id;
        private AudioSource voiceLine;      // temp
        /*
         * private AudioSource voiceLineEn;
         * private AudioSource voiceLineFr;
         * private AudioSource voiceLineEs;
         */
        
        public static void PlayHint()
        {
            HintIndex[Hints[_r.Next(Hints.Count)]].voiceLine.Play();
        }

    }
}