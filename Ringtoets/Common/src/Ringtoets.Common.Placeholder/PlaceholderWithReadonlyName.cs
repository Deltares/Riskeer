namespace Ringtoets.Common.Placeholder
{
    /// <summary>
    /// Simple placeholder object with only a name that cannot be changed.
    /// </summary>
    public class PlaceholderWithReadonlyName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaceholderWithReadonlyName"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public PlaceholderWithReadonlyName(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the placeholder.
        /// </summary>
        public string Name { get; private set; } 
    }
}