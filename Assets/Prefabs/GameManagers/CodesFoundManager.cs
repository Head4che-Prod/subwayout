using System;
using Hints;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.GameManagers
{
    public class CodesFoundManager : NetworkBehaviour
    {
        /// <summary>
        /// Instance of CodesFoundManager. Must be unique.
        /// </summary>
        private static CodesFoundManager _singleton;
        public static CodesFoundManager Singleton
        {
            get
            {
                if (_singleton == null)
                    Debug.LogWarning("CodesFoundManager has not been initialized.");
                return _singleton;
            }
            private set
            {
                if (_singleton == null)
                    _singleton = value;
                else
                    Debug.LogError("CodesFoundManager is already initialized.");
            }
        }

        public NetworkVariable<bool> BlueCodeFound { get; } = new NetworkVariable<bool>(false);
        public NetworkVariable<bool> RedCodeFound { get; } = new NetworkVariable<bool>(false);
        public NetworkVariable<bool> GreenCodeFound { get; } = new NetworkVariable<bool>(false);
        
        
        private void Start()
        {
            Singleton = this;
            BlueCodeFound.OnValueChanged += HandleHints;
            RedCodeFound.OnValueChanged += HandleHints;
            GreenCodeFound.OnValueChanged += HandleHints;
        }

        private void HandleHints(bool _, bool newVal)
        {
            if (newVal)
            {
                if (BlueCodeFound.Value && RedCodeFound.Value && GreenCodeFound.Value)
                {
                    HintSystem.EnableHints(Hint.Code);
                    HintSystem.DisableHints(Hint.Numbers);
                }
                else 
                    HintSystem.EnableHints(Hint.Numbers);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            BlueCodeFound.OnValueChanged -= HandleHints;
            RedCodeFound.OnValueChanged -= HandleHints;
            GreenCodeFound.OnValueChanged -= HandleHints;
        }
    }
}
