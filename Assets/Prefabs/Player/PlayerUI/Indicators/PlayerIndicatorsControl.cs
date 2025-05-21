using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Prefabs.Player.PlayerUI.Indicators
{
    public class PlayerIndicatorsControl : MonoBehaviour
    {
        private static PlayerIndicatorsControl _singleton;

        public static PlayerIndicatorsControl Singleton
        {
            get
            {
                if (_singleton)
                    return _singleton;
                return null;
            }
            private set
            {
                if (_singleton == null)
                    _singleton = value;
                else
                    Debug.LogError("PlayerIndicatorsControl singleton already set!");
            }
        }

        [SerializeField] private LocalizeStringEvent interactText;
        [SerializeField] private LocalizeStringEvent grabText;
        
        private void Start()
        {
            Singleton = this;
            interactText.SetEntry("controlHints.interact");
            grabText.SetEntry("controlHints.grab");
        }

        public static void SetGrabText(string entry) => Singleton?.grabText.SetEntry(entry);
        public static void SetInteractText(string entry) => Singleton?.interactText.SetEntry(entry);
        
        
        private void OnDestroy()
        {
            _singleton = null;
        }
    }
}