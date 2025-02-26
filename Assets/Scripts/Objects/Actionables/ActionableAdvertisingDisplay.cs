namespace Objects.Actionables
{
    public class ActionableAdvertisingDisplay : ObjectActionable
    {
        protected override ActionableType ActionableType => ActionableType.AdvertisingDisplay;

        /// <summary>
        /// Open the Advertising Display with a key.
        /// </summary>
        /// <remarks>See "Open ad box" - [Note 3]</remarks>
        protected override void Action()
        {
            throw new System.NotImplementedException();
        }
    }
}