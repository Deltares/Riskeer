using System.Collections.Generic;
using System.Linq;

using DelftTools.Shell.Core;

using Wti.Data;

namespace Wti.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="PipingData"/>
    /// in order to prepare it for performing a calculation.
    /// </summary>
    public class PipingCalculationInputs : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationInputs"/> class.
        /// </summary>
        public PipingCalculationInputs()
        {
            AvailablePipingSurfaceLines = Enumerable.Empty<RingtoetsPipingSurfaceLine>();
        }

        /// <summary>
        /// Gets or sets the piping data to be configured.
        /// </summary>
        public PipingData PipingData { get; set; }

        /// <summary>
        /// Gets or sets the available piping surface lines in order for the user to select
        /// one to set <see cref="Wti.Data.PipingData.SurfaceLine"/>.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> AvailablePipingSurfaceLines { get; set; }

        #region IObservable

        public void Attach(IObserver observer)
        {
            PipingData.Attach(observer);
        }

        public void Detach(IObserver observer)
        {
            PipingData.Detach(observer);
        }

        public void NotifyObservers()
        {
            PipingData.NotifyObservers();
        }

        #endregion
    }
}