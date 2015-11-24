using System.Collections.Generic;
using System.Linq;

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

        private static IEnumerable<PipingCalculationInputs> WrapCalculationsInPresentationObjects(PipingFailureMechanism failureMechanism)
        {
            return failureMechanism.Calculations.Select(calculation => new PipingCalculationInputs
            {
                PipingData = calculation,
                AvailablePipingSurfaceLines = failureMechanism.SurfaceLines,
                AvailablePipingSoilProfiles = failureMechanism.SoilProfiles
            });
        }
    }
}