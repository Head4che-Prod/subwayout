using UnityEngine;

namespace Prefabs.Puzzles.Airflow
{
    public class Flap : MonoBehaviour
    {
        private static readonly int IsOpen = Animator.StringToHash("isOpen");
        private Animator _animator;

        public void Start()
        {
            _animator = GetComponent<Animator>();
        }
        
        public void ChangeRotation(bool isOpen)
        {
            _animator.SetBool(IsOpen, isOpen);
        }
        
    }
}