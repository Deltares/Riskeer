namespace Ringtoets.HydraRing.Calculation.Data
{
    /// <summary>
    /// Container of all data necessary for performing an hydraulic data calculation via Hydra-Ring.
    /// </summary>
    public abstract class HydraulicCalculationData : HydraRingCalculationData
    {
        private readonly double beta;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraulicCalculationData"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="beta">The target reliability index to use during the calculation.</param>
        protected HydraulicCalculationData(int hydraulicBoundaryLocationId, double beta) : base(hydraulicBoundaryLocationId)
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