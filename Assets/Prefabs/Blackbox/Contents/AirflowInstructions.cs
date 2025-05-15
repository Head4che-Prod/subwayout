using Objects;
using Prefabs.Puzzles.Airflow;
using UnityEngine;

namespace Prefabs.Blackbox.Contents
{
    public class AirflowInstructions : ObjectGrabbable
    {
        private static AirflowInstructions _singleton;
        public static AirflowInstructions Singleton
        {
            get => _singleton;
            private set
            {
                if (_singleton == null)
                    _singleton = value;
                else
                    Debug.LogError("AirflowInstructions singleton already set!");
            }
        }

        public bool Grabbed { get; private set; } = false;
        
        public override void Start()
        {
            base.Start();
            Singleton = this;
        }

        public override void Grab()
        {
            Grabbed = true;
            
            if (WindowPuzzle.Singleton.IsVisible)
                Hints.HintSystem.DisableHints("BlueCode");
            
            base.Grab();
        }
    }
}