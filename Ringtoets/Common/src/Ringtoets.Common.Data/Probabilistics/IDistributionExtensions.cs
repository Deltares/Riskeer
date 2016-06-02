// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Probabilistics
{
    /// <summary>
    /// Extension for <see cref="IDistribution"/> to support variation coefficient.
    /// </summary>
    public static class IDistributionExtensions
    {
        /// <summary>
        /// Gets the variation coefficient (<see cref="IDistribution.StandardDeviation"/> / <see cref="IDistribution.Mean"/>) of the distribution.
        /// </summary>
        /// <param name="distribution">The distribution.</param>
        /// <returns>The variation coefficient.</returns>
        public static RoundedDouble GetVariationCoefficient(this IDistribution distribution)
        {
            return new RoundedDouble(distribution.Mean.NumberOfDecimalPlaces, distribution.Mean == 0 ?
                                                                                  double.PositiveInfinity :
                                                                                  distribution.StandardDeviation/distribution.Mean);
        }

        /// <summary>
        /// Sets the <see cref="IDistribution.StandardDeviation"/> of the distribution (<paramref name="variationCoefficient"/> * <see cref="IDistribution.Mean"/>).
        /// </summary>
        /// <param name="distribution">The distribution.</param>
        /// <param name="variationCoefficient">The variation coefficient.</param>
        /// <exception cref="ArgumentOutOfRangeException">Standard deviation is less than 0.</exception>
        public static void SetStandardDeviationFromVariationCoefficient(this IDistribution distribution, double variationCoefficient)
        {
            distribution.StandardDeviation = (RoundedDouble) variationCoefficient*distribution.Mean;
        }

        /// <summary>
        /// Sets the <see cref="IDistribution.Mean"/> of the distribution (<see cref="IDistribution.StandardDeviation"/> / <paramref name="variationCoefficient"/>).
        /// </summary>
        /// <param name="distribution">The distribution.</param>
        /// <param name="variationCoefficient">The variation coefficient.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="variationCoefficient"/> is less than or equal to 0.</exception>
        public static void SetMeanFromVariationCoefficient(this IDistribution distribution, double variationCoefficient)
        {
            if (variationCoefficient <= 0)
            {
                throw new ArgumentOutOfRangeException("variationCoefficient", Resources.VariationCoefficient_Should_be_greater_than_zero);
            }
            distribution.Mean = (RoundedDouble) (distribution.StandardDeviation/variationCoefficient);
        }
    }
}