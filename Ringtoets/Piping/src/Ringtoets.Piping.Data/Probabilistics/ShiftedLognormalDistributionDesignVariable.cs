namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// This class defines a design variable for a shifted lognormal distribution.
    /// </summary>
    public class ShiftedLognormalDistributionDesignVariable : DesignVariable<ShiftedLognormalDistribution>
    {
        private readonly ShiftedLognormalDistribution distribution;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftedLognormalDistributionDesignVariable"/> class.
        /// </summary>
        /// <param name="distribution">A shifted lognormal distribution.</param>
        public ShiftedLognormalDistributionDesignVariable(ShiftedLognormalDistribution distribution) : base(distribution)
        {
            this.distribution = distribution;
        }

        public override double GetDesignValue()
        {
            return new LognormalDistributionDesignVariable(Distribution)
            {
                Percentile = Percentile
            }.GetDesignValue() + distribution.Shift;
        }
    }
}