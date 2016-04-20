using System;

using Core.Common.Base.Data;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.Piping.Data.TestUtil
{
    /// <summary>
    /// Helper class for dealing with classes that have inherently limited precision.
    /// </summary>
    public static class LimitedPrecisionHelper
    {
        /// <summary>
        /// Gets the accuracy for a <see cref="RoundedDouble"/>.
        /// </summary>
        public static double GetAccuracy(this RoundedDouble roundedDouble)
        {
            return Math.Pow(10.0, -roundedDouble.NumberOfDecimalPlaces);
        }

        /// <summary>
        /// Gets the accuracy for a <see cref="IDistribution"/>.
        /// </summary>
        /// <remarks>Assumes that all the parameters of the distributions share the same accuracy.</remarks>
        public static double GetAccuracy(this IDistribution distribution)
        {
            return distribution.Mean.GetAccuracy();
        }
    }
}