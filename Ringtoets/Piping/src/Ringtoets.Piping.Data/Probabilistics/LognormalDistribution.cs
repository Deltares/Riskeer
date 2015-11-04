using System;

using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// Class representing a log-normal distribution.
    /// </summary>
    public class LognormalDistribution
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
    }
}