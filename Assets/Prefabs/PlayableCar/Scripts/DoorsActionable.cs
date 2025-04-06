using Objects;
using Prefabs.GameManagers;
using Prefabs.Player;

namespace Prefabs.PlayableCar.Scripts
{
    public class DoorsActionable : ObjectActionable
    {
        protected override void Action(PlayerObject player)
        {
            if (TutorialManager.Singleton.State == TutorialState.TrainStopped)
                TutorialManager.Singleton.State = TutorialState.TrainMoving;
        }
    }
}