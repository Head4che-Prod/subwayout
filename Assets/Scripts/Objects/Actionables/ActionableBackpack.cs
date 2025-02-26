namespace Objects.Actionables
{
    public class ActionableBackpack : ObjectActionable
    {
        protected override ActionableType ActionableType => ActionableType.Backpack;

        /// <summary>
        /// Open a backpack.
        /// </summary>
        /// <remarks>See "Search backpack" - [Note 3]</remarks>
        protected override void Action()
        {
            throw new System.NotImplementedException();
        }
    }
}