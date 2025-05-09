using System.Collections;
using Prefabs.Player.PlayerUI.DebugConsole;
using UnityEngine;
using UnityEngine.Serialization;

namespace Prefabs.Blackbox.Box
{
    public class BlackBoxLid : BlackBoxPart
    {
        [FormerlySerializedAs("_lidOpenAnimator")] [SerializeField] private Animator lidOpenAnimator;
        private static BlackBoxLid _singleton;

        public void Start()
        {
            _singleton = this;
            DebugConsole.AddCommand("openBlackBox", RaiseLid);
        }
        
        public static void RaiseLid()
        {
            _singleton.StartCoroutine(_singleton.OpenLidWhenAble());
        }

        private IEnumerator OpenLidWhenAble()
        {
            while (!blackBox.IsPulledOut)
                yield return null;
            
            // Only ever called once, no need to hash.
            _singleton.lidOpenAnimator.SetTrigger("open");
        }
    }
}
