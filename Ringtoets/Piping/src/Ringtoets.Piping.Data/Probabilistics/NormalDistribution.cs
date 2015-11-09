using System;

using MathNet.Numerics.Distributions;

using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// Class representing a normal (or Gaussian) distribution.
    /// </summary>
    public class NormalDistribution : IDistribution
    {
        private double standardDeviation;

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalDistribution"/> class,
        /// initialized as the standard normal distribution.
        /// </summary>
        public NormalDistribution()
        {
            Mean = 0.0;
            StandardDeviation = 1.0;
        }

        /// <summary>
        /// Gets or sets the mean (&#956;) of the distribution.
        /// </summary>
        public double Mean { get; set; }

        /// <summary>
        /// Gets or sets the standard deviation (&#963;) of the distribution.
        /// </summary>
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
                    throw new ArgumentException(Resources.StandardDeviation_Should_be_greater_than_zero);
                }
                standardDeviation = value;
            }
        }

        public double InverseCDF(double p)
        {
            if (p < 0.0 || p > 1)
            {
                throw new ArgumentOutOfRangeException("p", Resources.IDistribution_InverseCDF_Probability_must_be_in_range);
            }
            return Normal.InvCDF(Mean, StandardDeviation, p);
        }
    }
}