using System.Collections.Generic;
using System.Linq;

using Wti.Data;

namespace Wti.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="PipingData"/>
    /// in order to prepare it for performing a calculation.
    /// </summary>
    public class PipingCalculationInputs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationInputs"/> class.
        /// </summary>
        public PipingCalculationInputs()
        {
            AvailablePipingSurfaceLines = Enumerable.Empty<PipingSurfaceLine>();
        }

        /// <summary>
        /// Gets or sets the piping data to be configured.
        /// </summary>
        public PipingData PipingData { get; set; }

        /// <summary>
        /// Gets or sets the available piping surface lines in order for the user to select
        /// one to set <see cref="Wti.Data.PipingData.SurfaceLine"/>.
        /// </summary>
        public IEnumerable<PipingSurfaceLine> AvailablePipingSurfaceLines { get; set; }
    }
}