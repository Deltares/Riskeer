using System.Collections.Generic;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="PipingCalculation"/>
    /// in order to prepare it for performing a calculation.
    /// </summary>
    public class PipingCalculationContext : PipingContext<PipingCalculation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationContext"/> class.
        /// </summary>
        /// <param name="calculation">The <see cref="PipingCalculation"/> instance wrapped by this context object.</param>
        /// <param name="surfaceLines">The surface lines available within the piping context.</param>
        /// <param name="soilProfiles">The soil profiles available within the piping context.</param>
        public PipingCalculationContext(PipingCalculation calculation,
                                        IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines,
                                        IEnumerable<PipingSoilProfile> soilProfiles)
            : base(calculation, surfaceLines, soilProfiles) {}

        /// <summary>
        /// Clears the output of the <see cref="PipingCalculationContext"/>.
        /// </summary>
        public void ClearOutput()
        {
            WrappedData.ClearOutput();
        }
    }
}