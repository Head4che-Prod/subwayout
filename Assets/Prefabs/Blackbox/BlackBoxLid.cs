using UnityEngine;

namespace Prefabs.Blackbox
{
    public class BlackBoxLid : BlackBoxPart
    {
        [SerializeField] private Animator lidOpenAnimator;

        public void RaiseLid()
        {
            // Only ever called once, no need to hash.
            lidOpenAnimator.SetTrigger("open");
        }
    }
}
