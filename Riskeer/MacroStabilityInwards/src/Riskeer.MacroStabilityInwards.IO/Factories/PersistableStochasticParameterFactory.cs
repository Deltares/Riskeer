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
        /// <param name="isProbabilistic">Indicator whether the
        /// <see cref="PersistableStochasticParameter"/> is probabilistic.</param>
        /// <returns>The created <see cref="PersistableStochasticParameter"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/>
        /// is <c>null</c>.</exception>
        /// <remarks><paramref name="isProbabilistic"/> is default set to <c>true</c>.</remarks>
        public static PersistableStochasticParameter Create(IVariationCoefficientDistribution distribution, bool isProbabilistic = true)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            return new PersistableStochasticParameter
            {
                IsProbabilistic = isProbabilistic,
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