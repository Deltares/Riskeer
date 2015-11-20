using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.PresentationObjects
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
            AvailablePipingSoilProfiles = Enumerable.Empty<PipingSoilProfile>();
        }

        /// <summary>
        /// Gets or sets the piping data to be configured.
        /// </summary>
        public PipingCalculationData PipingData { get; set; }

        /// <summary>
        /// Gets or sets the available piping surface lines in order for the user to select
        /// one to set <see cref="Data.PipingCalculationData.SurfaceLine"/>.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> AvailablePipingSurfaceLines { get; set; }

        /// <summary>
        /// Gets or sets the available piping soil profiles in order for the user to select
        /// one to set <see cref="Data.PipingCalculationData.SoilProfile"/>.
        /// </summary>
        public IEnumerable<PipingSoilProfile> AvailablePipingSoilProfiles { get; set; }

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

        /// <summary>
        /// Clears the output of the <see cref="PipingCalculationInputs"/>.
        /// </summary>
        public void ClearOutput()
        {
            PipingData.ClearOutput();
        }
    }
}