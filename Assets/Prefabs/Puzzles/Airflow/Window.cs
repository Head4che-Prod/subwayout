using System;
using Objects;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.Airflow
{
    public class Window : MonoBehaviour, IObjectActionable
    {
        private static readonly int ClosedAnimationBoolean = Animator.StringToHash("IsClosed");

        [Header("Window Settings")]
        [SerializeField] private bool startClosed;
        [SerializeField] private Animator windowAnimator;

        public bool IsClosed { get; private set; }

        public void Awake()
        {
            IsClosed = startClosed;
        }
        public void Action()
        {
            AirflowGate.Singleton.ChangeWindowPosition(this);
        }

        public void ChangePosition(bool isClosed)
        {
            IsClosed = isClosed;
            windowAnimator.SetBool(ClosedAnimationBoolean, isClosed);
        }
    }
}