using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.FoldingSeats
{
    public class ChairsManager : MonoBehaviour
    {
        private readonly SingleChair[] _chairsBool = new SingleChair[24];
        private static ChairsManager _singleton;

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
        /// Checks the puzzle's wind condition.
        /// </summary>
        public void CheckChairs()
        {
            if (_chairsBool.All(c => c.IsInRightPosition))
            {
                Debug.Log("Win Chairs");
            }
        
        }
    }
}
