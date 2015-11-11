using System;

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

        public double Mean { get; set; }

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