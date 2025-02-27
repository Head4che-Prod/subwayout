using Objects;
using Prefabs.Player;

namespace Prefabs.Puzzles.HintSystem
{
    public class EmergencyCallTrigger : ObjectActionable
    {
        protected override void Action(PlayerObject _)
        {
            PuzzleHint.PlayHint();
        }
    }
}