namespace Objects.Actionables
{
    public class ActionableBackpack : ObjectActionable
    {
        public override ActionableType ActionableType => ActionableType.Backpack;

        /// <summary>
        /// Open a backpack.
        /// </summary>
        /// <remarks>See "Search backpack" - [Note 3]</remarks>
        public override void Action()
        {
            throw new System.NotImplementedException();
        }
    }
}