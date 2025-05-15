using System;
using UnityEngine;

namespace Debugger
{
    public class PositionDebugger : MonoBehaviour
    {
        private Vector3 _position;

        private void Start()
        {
            _position = transform.position;
        }

        private void Update()
        {
            if (transform.position != _position)
            {
                Debug.Log($"Object moved from {_position} to {transform.position}.");
                _position = transform.position;
            }
        }
    }
}