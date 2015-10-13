using System.Collections.Generic;
using System.Linq;

namespace Wti.Data
{
    /// <summary>
    /// Model for performing piping calculations.
    /// </summary>
    public class PipingFailureMechanism
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingFailureMechanism"/> class.
        /// </summary>
        public PipingFailureMechanism()
        {
            SurfaceLines = Enumerable.Empty<PipingSurfaceLine>();
            PipingData = new PipingData();
        }

        /// <summary>
        /// Gets the available surface lines within the scope of the piping failure mechanism.
        /// </summary>
        public IEnumerable<PipingSurfaceLine> SurfaceLines { get; private set; }

        /// <summary>
        /// Gets the input data which contains input and output of a piping calculation
        /// </summary>
        public PipingData PipingData { get; private set; }
    }
}