using Core.Common.Base;

using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Data.Properties;
using Ringtoets.Piping.Data;

namespace Ringtoets.Integration.Data
{
    /// <summary>
    /// The dike-based section to be assessed by the user for safety in regards of various failure mechanisms.
    /// </summary>
    public class DikeAssessmentSection : Observable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DikeAssessmentSection"/> class.
        /// </summary>
        public DikeAssessmentSection()
        {
            Name = Resources.DikeAssessmentSection_DisplayName;
            ReferenceLine = new InputPlaceholder(Resources.ReferenceLine_DisplayName);
            FailureMechanismContribution = new InputPlaceholder(Resources.FailureMechanismContribution_DisplayName);
            HydraulicBoundaryDatabase = new InputPlaceholder(Resources.HydraulicBoundaryDatabase_DisplayName);

            PipingFailureMechanism = new PipingFailureMechanism();
            GrassErosionFailureMechanism = new FailureMechanismPlaceholder(Resources.GrassErosionFailureMechanism_DisplayName);
            MacrostabilityInwardFailureMechanism = new FailureMechanismPlaceholder(Resources.MacrostabilityInwardFailureMechanism_DisplayName);
            OvertoppingFailureMechanism = new FailureMechanismPlaceholder(Resources.OvertoppingFailureMechanism_DisplayName);
            ClosingFailureMechanism = new FailureMechanismPlaceholder(Resources.ClosingFailureMechanism_DisplayName);
            FailingOfConstructionFailureMechanism = new FailureMechanismPlaceholder(Resources.FailingOfConstructionFailureMechanism_DisplayName);
            StoneRevetmentFailureMechanism = new FailureMechanismPlaceholder(Resources.StoneRevetmentFailureMechanism_DisplayName);
            AsphaltRevetmentFailureMechanism = new FailureMechanismPlaceholder(Resources.AsphaltRevetmentFailureMechanism_DisplayName);
            GrassRevetmentFailureMechanism = new FailureMechanismPlaceholder(Resources.GrassRevetmentFailureMechanism_DisplayName);
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
        /// Gets the "Piping" failure mechanism.
        /// </summary>
        public PipingFailureMechanism PipingFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Graserosie kruin en binnentalud" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder GrassErosionFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Macrostabiliteit binnenwaarts" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder MacrostabilityInwardFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Overslag en overloop" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder OvertoppingFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Niet sluiten" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder ClosingFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Constructief falen" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder FailingOfConstructionFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Steenbekledingen" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder StoneRevetmentFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Asfaltbekledingen" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder AsphaltRevetmentFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Grasbekledingen" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder GrassRevetmentFailureMechanism { get; private set; }
    }
}