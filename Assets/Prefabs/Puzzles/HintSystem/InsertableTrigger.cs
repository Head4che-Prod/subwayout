using Objects;

namespace Prefabs.Puzzles.HintSystem
{
    public class InsertableTrigger : ObjectGrabbable
    {
        public void Deactivate()
        {
            Drop();
            gameObject.SetActive(false);
        }
    }
}