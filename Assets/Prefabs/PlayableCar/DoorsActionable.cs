using Objects;
using Prefabs.GameManagers;

namespace Prefabs.PlayableCar
{
    public class DoorsActionable : IObjectActionable
    {
        public void Action()
        {
            if (TutorialManager.Instance.State == TutorialState.TrainStopped)
                TutorialManager.Instance.State = TutorialState.TrainMoving;
        }
    }
}