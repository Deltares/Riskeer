using System.Collections.Generic;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="PipingCalculationGroup"/>
    /// in order be able to create configurable piping calculations.
    /// </summary>
    public class PipingCalculationGroupContext : PipingContext<PipingCalculationGroup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationGroupContext"/> class.
        /// </summary>
        /// <param name="group">The <see cref="PipingCalculationGroup"/> instance wrapped by this context object.</param>
        /// <param name="surfaceLines">The surface lines available within the piping context.</param>
        /// <param name="soilProfiles">The soil profiles available within the piping context.</param>
        public PipingCalculationGroupContext(PipingCalculationGroup group,
                                             IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines,
                                             IEnumerable<PipingSoilProfile> soilProfiles) :
                                                 base(group, surfaceLines, soilProfiles)
        {
            
        }
    }
}