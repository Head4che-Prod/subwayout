namespace Objects.Actionables
{
    public class ActionableTrapdoor : ObjectActionable
    {
        protected override ActionableType ActionableType => ActionableType.Trapdoor;

        /// <summary>
        /// Open the trapdoor behind the ad.
        /// </summary>
        /// <remarks>See "Trapdoor behind ad" - [Note 3]</remarks>
        protected override void Action()
        {
            throw new System.NotImplementedException();
        }
    }
}