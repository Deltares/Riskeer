using Ringtoets.Common.Placeholder;

namespace Ringtoets.Integration.Data.Placeholders
{
    /// <summary>
    /// Defines a placeholder for input objects.
    /// </summary>
    public class InputPlaceholder : PlaceholderWithReadonlyName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputPlaceholder"/> class.
        /// </summary>
        /// <param name="name">The name of the placeholder.</param>
        public InputPlaceholder(string name) : base(name) {}
    }
}