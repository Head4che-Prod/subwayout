using System;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.Airflow
{
    public class Window : NetworkBehaviour
    {
        [SerializeField] private bool startClosed;
        [SerializeField] private Animator windowAnimator;
        
        private readonly NetworkVariable<bool> _isClosed = new NetworkVariable<bool>(false);
        public bool IsClosed => _isClosed.Value;

        public void Start()
        {
            
            
            if (IsServer)
                _isClosed.Value = startClosed;
        }
    }
}