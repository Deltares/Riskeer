using Ringtoets.Common.Placeholder;

namespace Ringtoets.Integration.Data.Placeholders
{
    /// <summary>
    /// Defines a placeholder for output objects.
    /// </summary>
    public class OutputPlaceholder : PlaceholderWithReadonlyName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputPlaceholder"/> class.
        /// </summary>
        /// <param name="name">The name of the placeholder.</param>
        public OutputPlaceholder(string name) : base(name) { }
    }
}