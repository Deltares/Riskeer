using System;

using MathNet.Numerics.Distributions;

using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// Class representing a log-normal distribution.
    /// </summary>
    public class LognormalDistribution : IDistribution
    {
        private double standardDeviation;

        /// <summary>
        /// Initializes a new instance of the <see cref="LognormalDistribution"/> class,
        /// initialized as the standard log-normal distribution.
        /// </summary>
        public LognormalDistribution()
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
                    throw new ArgumentException(Resources.StandardDeviation_Should_be_greater_then_zero);
                }
                standardDeviation = value;
            }
        }

        public virtual double InverseCDF(double p)
        {
            if (p < 0.0 || p > 1)
            {
                throw new ArgumentOutOfRangeException("p", "Kans moet in het bereik van [0, 1] opgegeven worden.");
            }
            return LogNormal.InvCDF(Mean, StandardDeviation, p);
        }
    }
}