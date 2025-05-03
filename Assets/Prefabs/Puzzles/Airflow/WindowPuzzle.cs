using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.Airflow
{
    public class WindowPuzzle : NetworkBehaviour
    {
        [SerializeField] private GameObject airflowGate;
        [SerializeField] private Window[] windows;

        private static WindowPuzzle _singleton;

        public static WindowPuzzle Singleton
        {
            get
            {
                if (_singleton != null)
                    return _singleton;
                Debug.LogError("AirflowGate singleton no set");
                return null;
            }
            private set
            {
                if (_singleton == null)
                    _singleton = value;
                else
                    Debug.LogError("AirflowGate singleton already set!");
            }
        }

        private Flap[] _flaps;
        private bool _isVisible = false;

        private bool[] _windowsClosed;
        private readonly Dictionary<Window, ushort> _windowIds = new Dictionary<Window, ushort>();

        public void Start()
        {
            Singleton = this;

            _flaps = new Flap[airflowGate.transform.childCount];
            for (ushort i = 0; i < _flaps.Length; i++)
                _flaps[i] = airflowGate.transform.GetChild(i).GetComponent<Flap>();

            // Initialise window Ids on all clients.
            // Due to being serialized the same way, these will be identical on all clients.
            for (ushort i = 0; i < windows.Length; i++)
                _windowIds.Add(windows[i], i);

            // Initialize NetworkVariables on host
            if (IsHost)
            {
                _windowsClosed = new bool[windows.Length];
                for (ushort i = 0; i < windows.Length; i++)
                    _windowsClosed[i] = windows[i].IsClosed;
            }
        }

        public void ChangeWindowPosition(Window window)
        {
            ChangeWindowPositionServerRpc(_windowIds[window]);
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void ChangeWindowPositionServerRpc(ushort windowId)
        {
            bool windowIsNowClosed = !_windowsClosed[windowId];
            _windowsClosed[windowId] = windowIsNowClosed;
            ChangeWindowPositionClientRpc(windowId, windowIsNowClosed);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ChangeWindowPositionClientRpc(ushort windowId, bool isClosed)
        {
            windows[windowId].ChangePosition(isClosed);
            CheckWin();
        }

        /// <summary>
        /// Checks the puzzle's win condition.
        /// </summary>
        public void CheckWin()
        {
            if (!_isVisible && windows.All(w => w.IsClosed))
            {
                _isVisible = true;
                foreach (Flap flap in _flaps)
                    flap.ChangeRotation(true);
            }
            else if (_isVisible && windows.Any(w => !w.IsClosed))
            {
                _isVisible = false;
                foreach (Flap flap in _flaps)
                    flap.ChangeRotation(false);
            }
        }
    }
}