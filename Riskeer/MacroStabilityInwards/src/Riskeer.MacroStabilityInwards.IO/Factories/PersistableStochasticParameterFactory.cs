using System;
using Components.Persistence.Stability.Data;
using Riskeer.Common.Data.Probabilistics;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableStochasticParameterFactory"/>.
    /// </summary>
    internal static class PersistableStochasticParameterFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="PersistableStochasticParameter"/>.
        /// </summary>
        /// <param name="distribution">The distribution to create the
        /// <see cref="PersistableStochasticParameter"/> for.</param>
        /// <returns>The created <see cref="PersistableStochasticParameter"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/>
        /// is <c>null</c>.</exception>
        public static PersistableStochasticParameter Create(IVariationCoefficientDistribution distribution)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            return new PersistableStochasticParameter
            {
                IsProbabilistic = true,
                Mean = distribution.Mean,
                StandardDeviation = GetStandardDeviation(distribution)
            };
        }

        private static double GetStandardDeviation(IVariationCoefficientDistribution distribution)
        {
            return distribution.Mean * distribution.CoefficientOfVariation;
        }
    }
}