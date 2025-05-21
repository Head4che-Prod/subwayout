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
                if (_singleton != null)
                    return _singleton;
                Debug.LogError("PlayerIndicatorsControl singleton no set");
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

        public LocalizeStringEvent interactText;
        public LocalizeStringEvent grabText;
        
        private void Start()
        {
            Singleton = this;
            interactText.SetEntry("controlHints.interact");
            grabText.SetEntry("controlHints.grab");
        }
        
        private void OnDestroy()
        {
            _singleton = null;
        }
    }
}