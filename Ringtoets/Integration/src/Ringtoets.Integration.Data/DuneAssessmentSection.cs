using Core.Common.Base;

using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Data.Properties;

namespace Ringtoets.Integration.Data
{
    /// <summary>
    /// The dune-based section to be assessed by the user for safety in regards of various failure mechanisms.
    /// </summary>
    public class DuneAssessmentSection : Observable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuneAssessmentSection"/> class.
        /// </summary>
        public DuneAssessmentSection()
        {
            Name = Resources.DuneAssessmentSection_DisplayName;
            ReferenceLine = new InputPlaceholder(Resources.ReferenceLine_DisplayName);
            FailureMechanismContribution = new InputPlaceholder(Resources.FailureMechanismContribution_DisplayName);
            HydraulicBoundaryDatabase = new InputPlaceholder(Resources.HydraulicBoundaryDatabase_DisplayName);

            DuneErosionFailureMechanism = new FailureMechanismPlaceholder(Resources.DuneErosionFailureMechanism_DisplayName);
        }

        /// <summary>
        /// Gets or sets the name of the dune assessment section.
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
        /// Gets the "Duin erosie" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder DuneErosionFailureMechanism { get; private set; }
    }
}