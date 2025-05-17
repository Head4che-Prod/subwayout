using Objects;
using UnityEngine;

namespace Prefabs.Blackbox.Box
{
    public class BlackBoxLid : ParentInteractable
    {
        [SerializeField] private Animator lidOpenAnimator;

        public bool IsOpen { get; private set; } = false;
        public void RaiseLid()
        {
            // Only ever called once, no need to hash.
            IsOpen = true;
            lidOpenAnimator.SetTrigger("open");
        }
    }
}
