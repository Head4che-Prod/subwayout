namespace Objects
{
    public interface IObjectGrabbable : IRaycastResponsive
    {
        /// <summary>
        /// Whether the object is currently able to be grabbed.
        /// </summary>
        public bool Grabbable { get; set; }
        
        /// <summary>
        /// Makes the player grab the targeted object.
        /// </summary>
        public void Grab();
        
        /// <summary>
        /// Makes the player drop the grabbed object.
        /// </summary>
        public void Drop();
    }
}