using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="WrappedPipingCalculation"/>
    /// in order to prepare it for performing a calculation.
    /// </summary>
    public class PipingCalculationContext : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationContext"/> class.
        /// </summary>
        public PipingCalculationContext()
        {
            AvailablePipingSurfaceLines = Enumerable.Empty<RingtoetsPipingSurfaceLine>();
            AvailablePipingSoilProfiles = Enumerable.Empty<PipingSoilProfile>();
        }

        /// <summary>
        /// Gets or sets the piping data to be configured.
        /// </summary>
        public PipingCalculation WrappedPipingCalculation { get; set; }

        /// <summary>
        /// Gets or sets the available piping surface lines in order for the user to select
        /// one to set <see cref="PipingInput.SurfaceLine"/>.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> AvailablePipingSurfaceLines { get; set; }

        /// <summary>
        /// Gets or sets the available piping soil profiles in order for the user to select
        /// one to set <see cref="Data.PipingInput.SoilProfile"/>.
        /// </summary>
        public IEnumerable<PipingSoilProfile> AvailablePipingSoilProfiles { get; set; }

        #region IObservable

        public void Attach(IObserver observer)
        {
            WrappedPipingCalculation.Attach(observer);
        }

        public void Detach(IObserver observer)
        {
            WrappedPipingCalculation.Detach(observer);
        }

        public void NotifyObservers()
        {
            WrappedPipingCalculation.NotifyObservers();
        }

        #endregion

        /// <summary>
        /// Clears the output of the <see cref="PipingCalculationContext"/>.
        /// </summary>
        public void ClearOutput()
        {
            WrappedPipingCalculation.ClearOutput();
        }
    }
}