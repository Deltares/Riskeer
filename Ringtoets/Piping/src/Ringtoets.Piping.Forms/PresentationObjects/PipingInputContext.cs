using System.Collections.Generic;
using System.Linq;

using Core.Common.Base;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// A presentation layer object wrapping an instance of <see cref="WrappedPipingInput"/>
    /// and allowing for selecting a surfaceline or soil profile based on data available
    /// in a piping failure mechanism.
    /// </summary>
    public class PipingInputContext : IObservable
    {
        public PipingInputContext()
        {
            AvailablePipingSurfaceLines = Enumerable.Empty<RingtoetsPipingSurfaceLine>();
            AvailablePipingSoilProfiles = Enumerable.Empty<PipingSoilProfile>();
        }

        /// <summary>
        /// Gets or sets the wrapped piping input parameters instance.
        /// </summary>
        public PipingInput WrappedPipingInput { get; set; }

        /// <summary>
        /// Gets or sets the available piping surface lines in order for the user to select
        /// one to set <see cref="PipingInput.SurfaceLine"/>.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> AvailablePipingSurfaceLines { get; set; }

        /// <summary>
        /// Gets or sets the available piping soil profiles in order for the user to select
        /// one to set <see cref="PipingInput.SoilProfile"/>.
        /// </summary>
        public IEnumerable<PipingSoilProfile> AvailablePipingSoilProfiles { get; set; }

        public void Attach(IObserver observer)
        {
            WrappedPipingInput.Attach(observer);
        }

        public void Detach(IObserver observer)
        {
            WrappedPipingInput.Detach(observer);
        }

        public void NotifyObservers()
        {
            WrappedPipingInput.NotifyObservers();
        }
    }
}