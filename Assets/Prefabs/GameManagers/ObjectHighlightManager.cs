using System.Collections.Generic;
using Objects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.GameManagers
{
    public class ObjectHighlightManager : MonoBehaviour
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
            private set
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

        public void Awake()
        {
            Singleton = this;
        }

        public void Start()
        {
            _actionHighlightHold = InputSystem.actions.FindAction("HighlightHold");
            _actionHighlightToggle = InputSystem.actions.FindAction("HighlightToggle");

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

        public static void RegisterHighlightableObject(ObjectOutline outline)
        {
            if (!Singleton._foundObjects.Contains(outline))
                RegisterHighlightableObjectClientRpc(outline);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private static void RegisterHighlightableObjectClientRpc(ObjectOutline outline)
        {
            Singleton._foundObjects.Add(outline);
        }


        [Rpc(SendTo.ClientsAndHost)]
        public static void ForgetHighlightableObjectClientRpc(ObjectOutline outline)
        {
            Singleton._foundObjects.Remove(outline);
        }

        public void OnDestroy()
        {
            _actionHighlightHold.started -= _handleHighlightHoldStart;
            _actionHighlightHold.canceled -= _handleHighlightHoldEnd;
            _actionHighlightToggle.performed -= _handleHighlightToggle;
        }
    }
}