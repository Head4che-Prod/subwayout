using UnityEngine;

namespace Objects
{
    public class ParentActionable: MonoBehaviour, IObjectActionable
    {
        public Behaviour parent;
        private IObjectActionable _actionableParent;

        private void Start()
        {
            if(!parent.TryGetComponent<IObjectActionable>(out _actionableParent))
                Debug.LogError($"{name}'s parent {parent.name} is not actionable.");
        }
        
        public void Action() => _actionableParent.Action();
    }
}