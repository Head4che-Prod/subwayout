using Objects;
using Prefabs.GameManagers;
using UnityEngine;

namespace Prefabs.PlayableCar
{
    public class DoorsInteractable : MonoBehaviour, IObjectInteractable
    {
        public void Action()
        {
            Debug.Log($"DoorsInteractable: {TutorialManager.Instance.State} state");
            if (TutorialManager.CanBeChanged && TutorialManager.Instance.State == TutorialState.TrainStopped)
                TutorialManager.Instance.State = TutorialState.TrainMoving;
        }
    }
}