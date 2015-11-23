using System.Collections.Generic;
using System.Linq;

using Core.Common.Base;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// A presentation layer object wrapping an instance of <see cref="WrappedPipingInputParameters"/>
    /// and allowing for selecting a surfaceline or soil profile based on data available
    /// in a piping failure mechanism.
    /// </summary>
    public class PipingInputParametersContext : IObservable
    {
        public PipingInputParametersContext()
        {
            AvailablePipingSurfaceLines = Enumerable.Empty<RingtoetsPipingSurfaceLine>();
            AvailablePipingSoilProfiles = Enumerable.Empty<PipingSoilProfile>();
        }

        /// <summary>
        /// Gets or sets the wrapped piping input parameters instance.
        /// </summary>
        public PipingInputParameters WrappedPipingInputParameters { get; set; }

        /// <summary>
        /// Gets or sets the available piping surface lines in order for the user to select
        /// one to set <see cref="PipingInputParameters.SurfaceLine"/>.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> AvailablePipingSurfaceLines { get; set; }

        /// <summary>
        /// Gets or sets the available piping soil profiles in order for the user to select
        /// one to set <see cref="PipingInputParameters.SoilProfile"/>.
        /// </summary>
        public IEnumerable<PipingSoilProfile> AvailablePipingSoilProfiles { get; set; }

        public void Attach(IObserver observer)
        {
            WrappedPipingInputParameters.Attach(observer);
        }

        public void Detach(IObserver observer)
        {
            WrappedPipingInputParameters.Detach(observer);
        }

        public void NotifyObservers()
        {
            WrappedPipingInputParameters.NotifyObservers();
        }
    }
}