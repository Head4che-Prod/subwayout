using Objects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Prefabs.Puzzles.AI.Cheese
{
    public class CheeseGrabbable : ObjectGrabbable
    {
        private Collider cageCollider;

        public override void Start()
        {
            cageCollider = CageManager.Singleton._triggerCollider;
            base.Start();
        }
        public override void Grab()
        {
            cageCollider.providesContacts = false;
            cageCollider.isTrigger = true;
            base.Grab();
        }
        
        public override void Drop()
        {
            cageCollider.providesContacts = true;
            cageCollider.isTrigger = false;
            base.Drop();
        }
    }
}