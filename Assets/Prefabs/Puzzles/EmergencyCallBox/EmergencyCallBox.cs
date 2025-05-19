using Objects;
using Prefabs.GameManagers;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;

namespace Prefabs.Puzzles.EmergencyCallBox
{
    public class EmergencyCallBox : MonoBehaviour, IObjectInteractable
    {
        [SerializeField] private InsertableTrigger insertableTrigger;
        [SerializeField] private EmergencyCallTrigger callTrigger;
        private ObjectGrabbable _triggerGrabbable;
        private readonly NetworkVariable<bool> _isAwaitingTrigger = new NetworkVariable<bool>(true);

        private void Start()
        {
            _triggerGrabbable = insertableTrigger.GetComponent<ObjectGrabbable>();
        }
        public void Action()
        {
            if (_isAwaitingTrigger.Value && (ObjectGrabbable)PlayerInteract.LocalPlayerInteract.GrabbedObject == _triggerGrabbable)
            {
                callTrigger.Activate();
                insertableTrigger.Deactivate();
                _isAwaitingTrigger.Value = true;
                TutorialManager.Instance.State = TutorialState.HintSystemUnlocked;
            }
        }
    }
}
