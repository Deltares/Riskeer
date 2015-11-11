using System;

using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// Class representing a log-normal distribution.
    /// </summary>
    public class LognormalDistribution : IDistribution
    {
        private double standardDeviation;
        private double mean;

        /// <summary>
        /// Initializes a new instance of the <see cref="LognormalDistribution"/> class,
        /// initialized as the standard log-normal distribution (mu=0, sigma=1).
        /// </summary>
        public LognormalDistribution()
        {
            // Simplified calculation mean and standard deviation given mu=0 and sigma=1.
            mean = Math.Exp(-0.5);
            StandardDeviation = Math.Sqrt((Math.Exp(1) - 1) * Math.Exp(1));
        }

        /// <summary>
        /// Gets or sets the mean (expected value, E(X)) of the distribution.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Expected value is less then or equal to 0.</exception>
        public double Mean
        {
            get
            {
                return mean;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.LognormalDistribution_Mean_must_be_greater_equal_to_zero);
                }
                mean = value;
            }
        }

        public double StandardDeviation
        {
            get
            {
                return standardDeviation;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.StandardDeviation_Should_be_greater_than_zero);
                }
                standardDeviation = value;
            }
        }
    }
}