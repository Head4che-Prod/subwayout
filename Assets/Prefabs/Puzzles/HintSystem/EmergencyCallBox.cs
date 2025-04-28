using Objects;
using Prefabs.GameManagers;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.HintSystem
{
    public class EmergencyCallBox : ObjectActionable
    {
        [SerializeField] private InsertableTrigger insertableTrigger;
        [SerializeField] private EmergencyCallTrigger callTrigger;
        private ObjectGrabbable _triggerGrabbable;
        private readonly NetworkVariable<bool> _isAwaitingTrigger = new NetworkVariable<bool>(true);

        private void Start()
        {
            _triggerGrabbable = insertableTrigger.GetComponent<ObjectGrabbable>();
        }
        protected override void Action(PlayerObject player)
        {
            if (_isAwaitingTrigger.Value && PlayerInteract.Singleton.GrabbedObject == _triggerGrabbable)
            {
                callTrigger.Activate();
                insertableTrigger.Deactivate();
                _isAwaitingTrigger.Value = true;
            }
        }
    }
}
