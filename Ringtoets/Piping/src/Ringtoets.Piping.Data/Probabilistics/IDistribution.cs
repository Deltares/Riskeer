using System;

namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// This object represents a probabilistic distribution.
    /// </summary>
    public interface IDistribution
    {
        /// <summary>
        /// Gets or sets the mean (expected value, E(X)) of the distribution.
        /// </summary>
        double Mean { get; set; }

        /// <summary>
        /// Gets or sets the standard deviation (square root of the Var(X)) of the distribution.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Standard deviation is less then or equal to 0.</exception>
        double StandardDeviation { get; set; }
    }
}