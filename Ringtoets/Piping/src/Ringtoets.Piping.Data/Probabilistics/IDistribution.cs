using System;

namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// This object represents a probabilistic distribution.
    /// </summary>
    public interface IDistribution
    {
        /// <summary>
        /// Performs the inverse Cumulative Density Function on the distribution, returning 
        /// the concrete realization corresponding with the given probability.
        /// </summary>
        /// <param name="p">The probability, for which P(X&lt;x) applies where x will be the returned result.</param>
        /// <returns>The concrete realization value.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="p"/> is not in the range [0.0, 1.0].</exception>
        double InverseCDF(double p);
    }
}