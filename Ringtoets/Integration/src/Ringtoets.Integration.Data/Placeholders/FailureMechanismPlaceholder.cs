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
        public FailureMechanismPlaceholder(string name) : base(name)
        {
            SectionDivisions = new InputPlaceholder("Vakindeling");
            Locations = new InputPlaceholder("Locaties");
            BoundaryConditions = new InputPlaceholder("Randvoorwaarden");
            AssessmentResult = new OutputPlaceholder("Oordeel");
        }

        /// <summary>
        /// Gets the subdivision of the assessment section for which this failure mechanism is calculating.
        /// </summary>
        public InputPlaceholder SectionDivisions { get; private set; }

        /// <summary>
        /// Gets the locations relevant for evaluating this failure mechanism.
        /// </summary>
        public InputPlaceholder Locations { get; private set; }

        /// <summary>
        /// Gets the boundary conditions applying to this failure mechanism.
        /// </summary>
        public InputPlaceholder BoundaryConditions { get; private set; }

        /// <summary>
        /// Gets the calculation results for this failure mechanism.
        /// </summary>
        public OutputPlaceholder AssessmentResult { get; private set; }
    }
}