namespace Objects.Actionables
{
    public class ActionableEmergencyTrigger: ObjectActionable
    {
        public override ActionableType ActionableType => ActionableType.EmergencyTrigger;

        /// <summary>
        /// Hint system.
        /// </summary>
        /// <remarks>See "Insert trigger in emergency call" - [Tutorial]</remarks>
        public override void Action()
        {
            throw new System.NotImplementedException();
        }
    }
}