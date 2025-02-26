namespace Objects.Actionables
{
    public class ActionableMetroDoors : ObjectActionable
    {
        public override ActionableType ActionableType => ActionableType.MetroDoors;

        /// <summary>
        /// Try to open doors unfortunately...
        /// </summary>
        /// <remarks>See "Player presses the 'Open doors' button" - [Tutorial]</remarks>
        public override void Action()
        {
            throw new System.NotImplementedException();
        }
    }
}