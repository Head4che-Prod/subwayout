namespace Objects.Actionables
{
    public class ActionableMetroDoors : ObjectActionable
    {
        protected override ActionableType ActionableType => ActionableType.MetroDoors;

        /// <summary>
        /// Try to open doors unfortunately...
        /// </summary>
        /// <remarks>See "Player presses the 'Open doors' button" - [Tutorial]</remarks>
        protected override void Action()
        {
            throw new System.NotImplementedException();
        }
    }
}