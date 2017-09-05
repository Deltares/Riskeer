// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.Common.Data.Probabilistics
{
    /// <summary>
    /// Factory for creating design variables based on probabilistic distributions.
    /// </summary>
    public static class SemiProbabilisticDesignValueFactory
    {
        /// <summary>
        /// Creates the design variable of a <see cref="VariationCoefficientLogNormalDistribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to create the design variable for.</param>
        /// <param name="percentile">The percentile used to derive a deterministic value based on the <paramref name="distribution"/>.</param>
        /// <returns>A new <see cref="VariationCoefficientLogNormalDistributionDesignVariable"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>.</exception>
        public static VariationCoefficientLogNormalDistributionDesignVariable CreateDesignVariable(
            VariationCoefficientLogNormalDistribution distribution, double percentile)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(distribution)
            {
                Percentile = percentile
            };
        }
    }
}