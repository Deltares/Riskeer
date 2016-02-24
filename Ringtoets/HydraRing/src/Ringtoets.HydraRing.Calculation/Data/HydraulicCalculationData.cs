using Ringtoets.HydraRing.Data;

namespace Ringtoets.HydraRing.Calculation.Data
{
    /// <summary>
    /// Container of all data necessary for performing an hydraulic data calculation via Hydra-Ring.
    /// </summary>
    public abstract class HydraulicCalculationData : HydraRingCalculationData
    {
        private readonly HydraulicBoundaryLocation hydraulicBoundaryLocation;
        private readonly double beta;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraulicCalculationData"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to perform the calculation for.</param>
        /// <param name="beta">The beta value to use during the calculation.</param>
        protected HydraulicCalculationData(HydraulicBoundaryLocation hydraulicBoundaryLocation, double beta)
        {
            this.hydraulicBoundaryLocation = hydraulicBoundaryLocation;
            this.beta = beta;
        }

        public override HydraulicBoundaryLocation HydraulicBoundaryLocation
        {
            get
            {
                return hydraulicBoundaryLocation;
            }
        }

        public override double Beta
        {
            get
            {
                return beta;
            }
        }
    }
}