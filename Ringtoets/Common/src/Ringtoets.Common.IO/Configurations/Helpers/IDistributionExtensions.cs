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
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.Common.IO.Configurations.Helpers
{
    /// <summary>
    /// Extension methods for converting <see cref="IDistribution"/> to <see cref="MeanStandardDeviationStochastConfiguration"/>.
    /// </summary>
    public static class IDistributionExtensions
    {
        /// <summary>
        /// Configure a new <see cref="MeanStandardDeviationStochastConfiguration"/> with 
        /// <see cref="MeanStandardDeviationStochastConfiguration.Mean"/> and 
        /// <see cref="MeanStandardDeviationStochastConfiguration.StandardDeviation"/> taken from
        /// <paramref name="distribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to take the values from.</param>
        /// <returns>A new <see cref="MeanStandardDeviationStochastConfiguration"/> with 
        /// <see cref="MeanStandardDeviationStochastConfiguration.Mean"/> and 
        /// <see cref="MeanStandardDeviationStochastConfiguration.StandardDeviation"/> set.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>.</exception>
        public static MeanStandardDeviationStochastConfiguration ToStochastConfiguration(this IDistribution distribution)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }
            return new MeanStandardDeviationStochastConfiguration
            {
                Mean = distribution.Mean,
                StandardDeviation = distribution.StandardDeviation
            };
        }

        /// <summary>
        /// Configure a new <see cref="MeanStandardDeviationStochastConfiguration"/> with 
        /// <see cref="MeanStandardDeviationStochastConfiguration.Mean"/> taken from
        /// <paramref name="distribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to take the values from.</param>
        /// <returns>A new <see cref="MeanStandardDeviationStochastConfiguration"/> with 
        /// <see cref="MeanStandardDeviationStochastConfiguration.Mean"/> set.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>.</exception>
        public static MeanStandardDeviationStochastConfiguration ToStochastConfigurationWithMean(this IDistribution distribution)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }
            return new MeanStandardDeviationStochastConfiguration
            {
                Mean = distribution.Mean
            };
        }

        /// <summary>
        /// Configure a new <see cref="MeanStandardDeviationStochastConfiguration"/> with 
        /// <see cref="MeanStandardDeviationStochastConfiguration.StandardDeviation"/> taken from
        /// <paramref name="distribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to take the values from.</param>
        /// <returns>A new <see cref="MeanStandardDeviationStochastConfiguration"/> with 
        /// <see cref="MeanStandardDeviationStochastConfiguration.StandardDeviation"/> set.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>.</exception>
        public static MeanStandardDeviationStochastConfiguration ToStochastConfigurationWithStandardDeviation(this IDistribution distribution)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }
            return new MeanStandardDeviationStochastConfiguration
            {
                StandardDeviation = distribution.StandardDeviation
            };
        }
    }
}