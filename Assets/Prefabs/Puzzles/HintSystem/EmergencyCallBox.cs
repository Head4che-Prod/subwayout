using Objects;
using Prefabs.Player;
using UnityEngine;

namespace Prefabs.Puzzles.HintSystem
{
    public class EmergencyCallBox : ObjectActionable
    {
        [SerializeField] private InsertableTrigger insertableTrigger;
        [SerializeField] private EmergencyCallTrigger callTrigger;
        private ObjectGrabbable _triggerGrabbable;
        private bool _triggerInserted;

        private void Start()
        {
            _triggerInserted = false;
            _triggerGrabbable = insertableTrigger.GetComponent<ObjectGrabbable>();
        }
        protected override void Action(PlayerObject player)
        {
            if (!_triggerInserted && _triggerGrabbable?.Owner == player)
            {
                callTrigger.Activate();
                insertableTrigger.Deactivate();
            }
        }
    }
}
