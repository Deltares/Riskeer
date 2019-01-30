// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using MathNet.Numerics.Distributions;

namespace Riskeer.Common.Data.Probabilistics
{
    /// <summary>
    /// Base calculator for design variables based on percentile and parameters of a distribution.
    /// </summary>
    internal abstract class PercentileBasedDesignVariableCalculator
    {
        /// <summary>
        /// Creates a base calculator.
        /// </summary>
        /// <param name="mean">The mean of the distribution.</param>
        /// <param name="standardDeviation">The standard deviation of the distribution.</param>
        /// <param name="coefficientOfVariation">The coefficient of variation of the distribution.</param>
        protected PercentileBasedDesignVariableCalculator(double mean, double standardDeviation, double coefficientOfVariation)
        {
            Mean = mean;
            StandardDeviation = standardDeviation;
            CoefficientOfVariation = coefficientOfVariation;
        }

        /// <summary>
        /// Gets the mean of the distribution.
        /// </summary>
        protected double Mean { get; }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        protected double StandardDeviation { get; }

        /// <summary>
        /// Gets the coefficient of variation of the distribution.
        /// </summary>
        protected double CoefficientOfVariation { get; }

        /// <summary>
        /// Determines the design value based on a 'normal space' expected value and standard deviation.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="standardDeviation">The standard deviation.</param>
        /// <param name="percentile">The percentile.</param>
        /// <returns>The design value.</returns>
        protected static double DetermineDesignValue(double expectedValue, double standardDeviation, double percentile)
        {
            // Design factor is determined using the 'probit function', which is the inverse
            // CDF function of the standard normal distribution. For more information see:
            // "Quantile function" https://en.wikipedia.org/wiki/Normal_distribution
            double designFactor = Normal.InvCDF(0.0, 1.0, percentile);
            return expectedValue + designFactor * standardDeviation;
        }

        /// <summary>
        /// Calculates and returns the design value.
        /// </summary>
        /// <param name="percentile">The percentile to obtain the value for.</param>
        /// <returns>The design value at the given <paramref name="percentile"/>.</returns>
        internal abstract double GetDesignValue(double percentile);
    }
}