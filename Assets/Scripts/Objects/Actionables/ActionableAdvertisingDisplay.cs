namespace Objects.Actionables
{
    public class ActionableAdvertisingDisplay : ObjectActionable
    {
        public override ActionableType ActionableType => ActionableType.AdvertisingDisplay;

        /// <summary>
        /// Open the Advertising Display with a key.
        /// </summary>
        /// <remarks>See "Open ad box" - [Note 3]</remarks>
        public override void Action()
        {
            throw new System.NotImplementedException();
        }
    }
}