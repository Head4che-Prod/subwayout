using UnityEngine;

namespace Objects
{
    public class ParentInteractable: MonoBehaviour, IObjectInteractable
    {
        public Behaviour parent;
        private IObjectInteractable _interactableParent;

        private void Start()
        {
            if(!parent.TryGetComponent<IObjectInteractable>(out _interactableParent))
                Debug.LogError($"{name}'s parent {parent.name} is not interactable.");
        }
        
        public void Action() => _interactableParent.Action();
    }
}