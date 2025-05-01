using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Prefabs.Puzzles.Airflow
{
    public class AirflowGate : MonoBehaviour
    {
        [SerializeField] private Window[] windows;
        
        private static AirflowGate _singleton;
        
        public static AirflowGate Singleton
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
        
        private readonly Flap[] _flaps = new Flap[5];
        private bool _isVisible = false;

        public void Start()
        {
            Singleton = this;
            for (int i = 0; i < _flaps.Length; i++)
                _flaps[i] = transform.GetChild(i).GetComponent<Flap>();
        }
        
        /// <summary>
        /// Checks the puzzle's wind condition.
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
