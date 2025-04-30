using System;
using System.Collections.Generic;
using Objects;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.GameManagers
{
    public class ObjectHighlightManager : NetworkBehaviour
    {
        private static ObjectHighlightManager _singleton;

        public static ObjectHighlightManager Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    Debug.LogWarning("Object highlight manager singleton has not been initialized.");
                }
                return _singleton;
            }
            set
            {
                if (_singleton == null)
                {
                    _singleton = value;
                }
                else
                {
                    Debug.LogError("Object highlight manager singleton already initialized.");
                }
            }
        }


        private bool _highlightHeld = false;
        private bool _highlightToggled = false;
        private System.Action<InputAction.CallbackContext> _handleHighlightHoldEnd;
        private System.Action<InputAction.CallbackContext> _handleHighlightHoldStart;
        private System.Action<InputAction.CallbackContext> _handleHighlightToggle;

        private bool HighlightHeld
        {
            set
            {
                _highlightHeld = value;
                UpdateHighlight();
            }
        }

        private void HighlightToggle()
        {
            _highlightToggled = !_highlightToggled;
            UpdateHighlight();
        }

        public static bool HighlightEnabled => Singleton._highlightHeld ^ Singleton._highlightToggled;

        private InputAction _actionHighlightHold;
        private InputAction _actionHighlightToggle;

        private readonly HashSet<ObjectOutline> _foundObjects = new HashSet<ObjectOutline>();

        private ObjectOutline GetOutline(ulong objectId) => NetworkManager.Singleton.SpawnManager
            .SpawnedObjects[objectId].GetComponent<ObjectOutline>();
        
        public void Awake()
        {
            Singleton = this;
        }

        public void Start()
        {
            _actionHighlightHold = PlayerObject.LocalPlayer.Input.actions.FindAction("HighlightHold");
            _actionHighlightToggle = PlayerObject.LocalPlayer.Input.actions.FindAction("HighlightToggle");

            _handleHighlightHoldStart = _ => HighlightHeld = true;
            _actionHighlightHold.started += _handleHighlightHoldStart;
            _handleHighlightHoldEnd = _ => HighlightHeld = false;
            _actionHighlightHold.canceled += _handleHighlightHoldEnd;
            _handleHighlightToggle = _ => HighlightToggle();
            _actionHighlightToggle.performed += _handleHighlightToggle;
        }

        private void UpdateHighlight()
        {
            bool highlight = HighlightEnabled;
            foreach (ObjectOutline outline in _foundObjects)
                outline.enabled = highlight;
        }

        public static void RegisterHighlightableObject(ulong objectId) => Singleton.RegisterHighlightableObjectClientRpc(objectId);

        [Rpc(SendTo.ClientsAndHost)]
        private void RegisterHighlightableObjectClientRpc(ulong objectId)
        {
            Singleton._foundObjects.Add(GetOutline(objectId));
        }

        public static void ForgetHighlightableObject(ulong objectId) => Singleton.ForgetHighlightableObjectClientRpc(objectId);
        
        [Rpc(SendTo.ClientsAndHost)]
        private void ForgetHighlightableObjectClientRpc(ulong objectId)
        {
            Singleton._foundObjects.Remove(GetOutline(objectId));
        }

        public override void OnDestroy()
        {
            _actionHighlightHold.started -= _handleHighlightHoldStart;
            _actionHighlightHold.canceled -= _handleHighlightHoldEnd;
            _actionHighlightToggle.performed -= _handleHighlightToggle;
        }
    }
}