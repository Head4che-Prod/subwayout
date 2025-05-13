using Objects;
using Prefabs.GameManagers;
using UnityEngine;

namespace Prefabs.PlayableCar
{
    public class DoorsActionable : MonoBehaviour, IObjectActionable
    {
        public void Action()
        {
            Debug.Log($"DoorsActionable: {TutorialManager.Instance.State} state");
            if (TutorialManager.Instance.State == TutorialState.TrainStopped)
                TutorialManager.Instance.State = TutorialState.TrainMoving;
        }
    }
}