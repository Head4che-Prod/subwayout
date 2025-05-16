using Hints;
using Objects;

namespace Prefabs.Blackbox.Contents
{
    public class AirflowInstructions : ObjectGrabbable
    {
        public override void Grab()
        {
            HintSystem.DisableHints(Hint.BlueCode);
            base.Grab();
        }
    }
}
