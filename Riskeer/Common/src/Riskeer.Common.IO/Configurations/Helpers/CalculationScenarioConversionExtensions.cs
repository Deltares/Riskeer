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
using Riskeer.Common.Data.Calculation;

namespace Riskeer.Common.IO.Configurations.Helpers
{
    /// <summary>
    /// Extension methods for converting <see cref="ICalculationScenario"/> to <see cref="ScenarioConfiguration"/>.
    /// </summary>
    public static class CalculationScenarioConversionExtensions
    {
        /// <summary>
        /// Configure a new <see cref="ScenarioConfiguration"/> with 
        /// <see cref="ScenarioConfiguration.Contribution"/> and 
        /// <see cref="ScenarioConfiguration.IsRelevant"/> taken from
        /// <paramref name="calculationScenario"/>.
        /// </summary>
        /// <param name="calculationScenario">The calculation scenario to take the values from.</param>
        /// <returns>A new <see cref="ScenarioConfiguration"/> with 
        /// <see cref="ScenarioConfiguration.Contribution"/> and 
        /// <see cref="ScenarioConfiguration.IsRelevant"/> set.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationScenario"/> is <c>null</c>.</exception>
        public static ScenarioConfiguration ToScenarioConfiguration(this ICalculationScenario calculationScenario)
        {
            if (calculationScenario == null)
            {
                throw new ArgumentNullException(nameof(calculationScenario));
            }

            return new ScenarioConfiguration
            {
                Contribution = calculationScenario.Contribution * 100,
                IsRelevant = calculationScenario.IsRelevant
            };
        }
    }
}