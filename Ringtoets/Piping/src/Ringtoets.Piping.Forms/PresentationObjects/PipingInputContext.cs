using System.Collections.Generic;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// A presentation layer object wrapping an instance of <see cref="WrappedPipingInput"/>
    /// and allowing for selecting a surfaceline or soil profile based on data available
    /// in a piping failure mechanism.
    /// </summary>
    public class PipingInputContext : PipingContext<PipingInput>
    {
        public PipingInputContext(PipingInput input, IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, IEnumerable<PipingSoilProfile> profiles):
            base(input, surfaceLines, profiles){ }
    }
}