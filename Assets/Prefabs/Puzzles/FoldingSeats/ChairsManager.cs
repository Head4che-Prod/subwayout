using System.Linq;
using UnityEngine;

namespace Prefabs.Puzzles.FoldingSeats
{
    public class ChairsManager : MonoBehaviour
    {
        private readonly SingleChair[] _chairsBool = new SingleChair[24];
        private static ChairsManager _singleton;

        private bool _gameWon = false;
        
        public static ChairsManager Singleton
        {
            get
            {
                if (_singleton != null)
                    return _singleton;
                Debug.LogError("ChairsManager singleton no set");
                return null;
            }
            private set
            {
                if (_singleton == null)
                    _singleton = value;
                else 
                    Debug.LogError("ChairsManager singleton already set!");
            }
        }

        void Start()
        {
            Singleton = this;
            for (int i = 0; i < 24; i++)
            {
                _chairsBool[i] = transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<SingleChair>(); //GetChild(0) 2 times because we search for "bottom" and the Find("bottom" doesn't work
            }
        }

        /// <summary>
        /// Returns whether the chairs are in the correct position.
        /// </summary>
        public bool CheckChairs()
        {
            if (!_gameWon && _chairsBool.All(chair => chair.IsInRightPosition))
            {
                _gameWon = true;
                return true;
            }
            return false;
        }
    }
}
