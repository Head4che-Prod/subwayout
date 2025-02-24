namespace Objects.Actionables
{
    public class ActionableEmergencyTrigger: ObjectActionable
    {
        public override ActionableType ActionableType => ActionableType.EmergencyTrigger;

        public override void Action()
        {
            throw new System.NotImplementedException();
        }
    }
}