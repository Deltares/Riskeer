using Ringtoets.Common.Data;
using Ringtoets.Common.Placeholder;

namespace Ringtoets.Integration.Data.Placeholders
{
    /// <summary>
    /// Defines a placeholder for unimplemented failure mechanisms objects
    /// </summary>
    public class FailureMechanismPlaceholder : PlaceholderWithReadonlyName, IFailureMechanism
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMechanismPlaceholder"/> class.
        /// </summary>
        /// <param name="name">The placeholder's name.</param>
        public FailureMechanismPlaceholder(string name) : base(name) {}
    }
}