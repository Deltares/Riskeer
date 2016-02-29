using Ringtoets.HydraRing.Calculation.Common;

namespace Ringtoets.HydraRing.Calculation.Data
{
    /// <summary>
    /// Container of all data necessary for performing an hydraulic data calculation via Hydra-Ring.
    /// </summary>
    public abstract class HydraulicCalculation : HydraRingCalculation
    {
        private readonly double beta;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraulicCalculation"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="beta">The target reliability index to use during the calculation.</param>
        protected HydraulicCalculation(int hydraulicBoundaryLocationId, double beta) : base(hydraulicBoundaryLocationId)
        {
            this.beta = beta;
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