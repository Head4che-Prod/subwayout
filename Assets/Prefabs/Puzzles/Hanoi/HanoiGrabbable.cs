namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiGrabbable : ObjectGrabbable
    {
        public override void Drop()
        {
            base.Drop();
            
            HanoiCollider.ResetBall(Rb);
        }
    }
}