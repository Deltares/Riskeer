using System.Collections.Generic;

using Core.Common.Base;

using Ringtoets.Common.Data;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Data.Properties;

namespace Ringtoets.Integration.Data
{
    /// <summary>
    /// Base implementation of assessment sections.
    /// </summary>
    public abstract class AssessmentSectionBase : Observable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSectionBase"/> class.
        /// </summary>
        protected AssessmentSectionBase()
        {
            Name = "";
            ReferenceLine = new InputPlaceholder(Resources.ReferenceLine_DisplayName);
            FailureMechanismContribution = new InputPlaceholder(Resources.FailureMechanismContribution_DisplayName);
            HydraulicBoundaryDatabase = new InputPlaceholder(Resources.HydraulicBoundaryDatabase_DisplayName);
        }

        /// <summary>
        /// Gets or sets the name of the assessment section.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the reference line defining the geometry of the dike assessment section.
        /// </summary>
        public InputPlaceholder ReferenceLine { get; private set; }

        /// <summary>
        /// Gets or sets the contribution of each failure mechanism available in this assessment section.
        /// </summary>
        public InputPlaceholder FailureMechanismContribution { get; private set; }

        /// <summary>
        /// Gets or sets the hydraulic boundary database.
        /// </summary>
        public InputPlaceholder HydraulicBoundaryDatabase { get; private set; }

        /// <summary>
        /// Gets the failure mechanisms corresponding to the assessment section.
        /// </summary>
        public abstract IEnumerable<IFailureMechanism> GetFailureMechanisms();
    }
}