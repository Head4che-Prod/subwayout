using System;
using Objects;
using UnityEngine;

namespace Prefabs.Puzzles.Airflow
{
    public class Window : MonoBehaviour, IObjectInteractable
    {
        private static readonly int ClosedAnimationBoolean = Animator.StringToHash("IsClosed");

        [Header("Window Settings")]
        [SerializeField] private bool startClosed;
        [SerializeField] private Animator windowAnimator;

        public bool IsClosed { get; private set; }

        public void Awake()
        {
            ChangePosition(startClosed);
        }

        public void Action()
        {
            WindowPuzzle.Singleton.ChangeWindowPosition(this);
        }

        public void ChangePosition(bool isClosed)
        {
            IsClosed = isClosed;
            windowAnimator.SetBool(ClosedAnimationBoolean, isClosed);
        }
    }
}