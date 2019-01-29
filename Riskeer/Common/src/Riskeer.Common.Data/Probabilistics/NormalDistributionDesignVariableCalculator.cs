// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

namespace Riskeer.Common.Data.Probabilistics
{
    /// <summary>
    /// Calculator for design variables for normal distributions.
    /// </summary>
    internal class NormalDistributionDesignVariableCalculator : PercentileBasedDesignVariableCalculator
    {
        private NormalDistributionDesignVariableCalculator(double mean, double standardDeviation, double coefficientOfVariation)
            : base(mean, standardDeviation, coefficientOfVariation) {}

        /// <summary>
        /// Creates a <see cref="PercentileBasedDesignVariableCalculator"/> for normal distributions based
        /// on mean and standard deviation.
        /// </summary>
        /// <param name="mean">The mean of the normal distribution.</param>
        /// <param name="standardDeviation">The standard deviation of the normal distribution.</param>
        /// <returns>A new <see cref="PercentileBasedDesignVariableCalculator"/>.</returns>
        internal static PercentileBasedDesignVariableCalculator CreateWithStandardDeviation(double mean, double standardDeviation)
        {
            return new NormalDistributionDesignVariableCalculator(mean, standardDeviation, standardDeviation / mean);
        }

        /// <summary>
        /// Creates a <see cref="PercentileBasedDesignVariableCalculator"/> for normal distributions based
        /// on mean and coefficient of variation.
        /// </summary>
        /// <param name="mean">The mean of the normal distribution.</param>
        /// <param name="coefficientOfVariation">The coefficient of variation of the normal distribution.</param>
        /// <returns>A new <see cref="PercentileBasedDesignVariableCalculator"/>.</returns>
        internal static PercentileBasedDesignVariableCalculator CreateWithCoefficientOfVariation(double mean, double coefficientOfVariation)
        {
            return new NormalDistributionDesignVariableCalculator(mean, mean * coefficientOfVariation, coefficientOfVariation);
        }

        internal override double GetDesignValue(double percentile)
        {
            return DetermineDesignValue(Mean, StandardDeviation, percentile);
        }
    }
}