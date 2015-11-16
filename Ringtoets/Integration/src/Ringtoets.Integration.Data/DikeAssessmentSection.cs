using Ringtoets.Common.Placeholder;
using Ringtoets.Piping.Data;

namespace Ringtoets.Integration.Data
{
    /// <summary>
    /// The section to be assessed by the user for safety in regards of various failure mechanisms.
    /// </summary>
    public class DikeAssessmentSection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DikeAssessmentSection"/> class.
        /// </summary>
        public DikeAssessmentSection()
        {
            Name = "Dijktraject";
            ReferenceLine = new PlaceholderWithReadonlyName("Referentielijn");
            FailureMechanismContribution = new PlaceholderWithReadonlyName("Faalkansverdeling");
            HydraulicBoundaryDatabase = new PlaceholderWithReadonlyName("HR locatiedatabase");

            PipingFailureMechanism = new PipingFailureMechanism();
            GrassErosionFailureMechanism = new PlaceholderWithReadonlyName("Dijken - Graserosie kruin en binnentalud");
            MacrostabilityInwardFailureMechanism = new PlaceholderWithReadonlyName("Dijken - Macrostabiliteit binnenwaarts");
            OvertoppingFailureMechanism = new PlaceholderWithReadonlyName("Kunstwerken - Overslag en overloop");
            ClosingFailureMechanism = new PlaceholderWithReadonlyName("Kunstwerken - Niet sluiten");
            FailingOfConstructionFailureMechanism = new PlaceholderWithReadonlyName("Kunstwerken - Constructief falen");
            StoneRevetmentFailureMechanism = new PlaceholderWithReadonlyName("Kunstwerken - Steenbekledingen");
            AsphaltRevetmentFailureMechanism = new PlaceholderWithReadonlyName("Kunstwerken - Asfaltbekledingen");
            GrassRevetmentFailureMechanism = new PlaceholderWithReadonlyName("Kunstwerken - Grasbekledingen");
        }

        /// <summary>
        /// Gets or sets the name of the assessment section.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the reference line defining the geometry of the dike assessment section.
        /// </summary>
        public PlaceholderWithReadonlyName ReferenceLine { get; private set; }

        /// <summary>
        /// Gets or sets the contribution of each failure mechanism available in this assessment section.
        /// </summary>
        public PlaceholderWithReadonlyName FailureMechanismContribution { get; private set; }

        /// <summary>
        /// Gets or sets the hydraulic boundary database.
        /// </summary>
        public PlaceholderWithReadonlyName HydraulicBoundaryDatabase { get; private set; }

        /// <summary>
        /// Gets the "Piping" failure mechanism.
        /// </summary>
        public PipingFailureMechanism PipingFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Graserosie kruin en binnentalud" failure mechanism.
        /// </summary>
        public PlaceholderWithReadonlyName GrassErosionFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Macrostabiliteit binnenwaarts" failure mechanism.
        /// </summary>
        public PlaceholderWithReadonlyName MacrostabilityInwardFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Overslag en overloop" failure mechanism.
        /// </summary>
        public PlaceholderWithReadonlyName OvertoppingFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Niet sluiten" failure mechanism.
        /// </summary>
        public PlaceholderWithReadonlyName ClosingFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Constructief falen" failure mechanism.
        /// </summary>
        public PlaceholderWithReadonlyName FailingOfConstructionFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Steenbekledingen" failure mechanism.
        /// </summary>
        public PlaceholderWithReadonlyName StoneRevetmentFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Asfaltbekledingen" failure mechanism.
        /// </summary>
        public PlaceholderWithReadonlyName AsphaltRevetmentFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Grasbekledingen" failure mechanism.
        /// </summary>
        public PlaceholderWithReadonlyName GrassRevetmentFailureMechanism { get; private set; }
    }
}