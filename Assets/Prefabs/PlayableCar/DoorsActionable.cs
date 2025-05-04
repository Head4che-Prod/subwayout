using Prefabs.GameManagers;
using Prefabs.Player;

namespace Prefabs.PlayableCar
{
    public class DoorsActionable : Objects.ObjectActionable
    {
        protected override void Action(PlayerObject player)
        {
            if (TutorialManager.Instance.State == TutorialState.TrainStopped)
                TutorialManager.Instance.State = TutorialState.TrainMoving;
        }
    }
}