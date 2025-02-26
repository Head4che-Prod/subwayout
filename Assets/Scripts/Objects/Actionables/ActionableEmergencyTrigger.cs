namespace Objects.Actionables
{
    public class ActionableEmergencyTrigger: ObjectActionable
    {
        protected override ActionableType ActionableType => ActionableType.EmergencyTrigger;

        /// <summary>
        /// Hint system.
        /// </summary>
        /// <remarks>See "Insert trigger in emergency call" - [Tutorial]</remarks>
        protected override void Action()
        {
            throw new System.NotImplementedException();
        }
    }
}