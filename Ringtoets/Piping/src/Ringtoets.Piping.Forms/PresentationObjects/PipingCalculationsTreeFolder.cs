using System.Collections;

using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// This class represents a folder in the project-tree that holds all piping calculations
    /// for a piping failure mechanism.
    /// </summary>
    public class PipingCalculationsTreeFolder : CategoryTreeFolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationsTreeFolder"/> class.
        /// </summary>
        /// <param name="name">The name of the folder.</param>
        /// <param name="failureMechanism">The parent failure mechanism.</param>
        public PipingCalculationsTreeFolder(string name, PipingFailureMechanism failureMechanism) : base(name, WrapCalculationsInPresentationObjects(failureMechanism))
        {
            ParentFailureMechanism = failureMechanism;
        }

        public PipingFailureMechanism ParentFailureMechanism { get; private set; }

        private static IEnumerable WrapCalculationsInPresentationObjects(PipingFailureMechanism failureMechanism)
        {
            foreach (var calculationItem in failureMechanism.Calculations)
            {
                var pipingCalculation = calculationItem as PipingCalculation;
                var pipingCalculationGroup = calculationItem as PipingCalculationGroup;
                if (pipingCalculation != null)
                {
                    yield return new PipingCalculationContext(pipingCalculation,
                                                              failureMechanism.SurfaceLines,
                                                              failureMechanism.SoilProfiles);
                }
                else if (pipingCalculationGroup != null)
                {
                    yield return new PipingCalculationGroupContext(pipingCalculationGroup,
                                                                   failureMechanism.SurfaceLines,
                                                                   failureMechanism.SoilProfiles);
                }
                else
                {
                    yield return calculationItem;
                }
            }
        }
    }
}