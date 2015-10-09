namespace DelftTools.Utils
{
    /// <summary>
    /// Types implementing this interface have a name.
    /// </summary>
    public interface INameable
    {
        /// <summary>
        /// Gets or sets the name of this object.
        /// </summary>
        string Name { get; set; }
    }
}