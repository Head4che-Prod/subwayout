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

        private static ObjectHighlightManager Singleton
        {
            get
            {
                if (!_singleton)
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


        private static bool _highlightHeld = false;
        private static bool _highlightToggled = false;
        private static Action<InputAction.CallbackContext> _handleHighlightHoldEnd;
        private static Action<InputAction.CallbackContext> _handleHighlightHoldStart;
        private static Action<InputAction.CallbackContext> _handleHighlightToggle;

        private static bool HighlightHeld
        {
            set
            {
                _highlightHeld = value;
                UpdateHighlight();
            }
        }

        private static void HighlightToggle()
        {
            _highlightToggled = !_highlightToggled;
            UpdateHighlight();
        }

        public static bool HighlightEnabled => _highlightHeld ^ _highlightToggled;

        private static InputAction _actionHighlightHold;
        private static InputAction _actionHighlightToggle;

        private static readonly HashSet<ObjectOutline> FoundObjects = new HashSet<ObjectOutline>();

        private ObjectOutline GetOutline(ulong objectId) => NetworkManager.Singleton.SpawnManager
            .SpawnedObjects[objectId].GetComponent<ObjectOutline>();

        public void Awake()
        {
            Singleton = this;
        }

        public static void Init()
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

        private static void UpdateHighlight()
        {
            bool highlight = HighlightEnabled;
            foreach (ObjectOutline outline in FoundObjects)
                outline.enabled = highlight;
        }

        public static void RegisterHighlightableObject(ulong objectId) =>
            Singleton.RegisterHighlightableObjectClientRpc(objectId);

        [Rpc(SendTo.ClientsAndHost)]
        private void RegisterHighlightableObjectClientRpc(ulong objectId)
        {
            FoundObjects.Add(GetOutline(objectId));
        }

        public static void ForgetHighlightableObject(ulong objectId) =>
            Singleton.ForgetHighlightableObjectClientRpc(objectId);

        [Rpc(SendTo.ClientsAndHost)]
        private void ForgetHighlightableObjectClientRpc(ulong objectId)
        {
            FoundObjects.Remove(GetOutline(objectId));
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _actionHighlightHold.started -= _handleHighlightHoldStart;
            _actionHighlightHold.canceled -= _handleHighlightHoldEnd;
            _actionHighlightToggle.performed -= _handleHighlightToggle;
        }
    }
}