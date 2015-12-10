using Ringtoets.Common.Data;

namespace Ringtoets.Common.Placeholder
{
    /// <summary>
    /// Simple placeholder object with only a name that cannot be changed.
    /// </summary>
    public class PlaceholderWithReadonlyName : BaseFailureMechanism
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaceholderWithReadonlyName"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public PlaceholderWithReadonlyName(string name)
        {
            Name = name;
        }
    }
}