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

using System;
using Core.Common.Base.Data;
using Ringtoets.MacroStabilityInwards.CalculatedInput;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Class responsible for calculating the derived macro stability inwards input.
    /// </summary>
    public static class DerivedMacroStabilityInwardsInput
    {
        /// <summary>
        /// Gets the calculated waternet for extreme circumstances.
        /// </summary>
        /// <param name="input">The input to calculate the waternet for.</param>
        /// <param name="assessmentLevel">The assessment level at stake.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <returns>Returns the corresponding derived waternet value.</returns>
        public static MacroStabilityInwardsWaternet GetWaternetExtreme(MacroStabilityInwardsInput input, RoundedDouble assessmentLevel)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return input.SoilProfileUnderSurfaceLine != null
                       ? WaternetCalculationService.CalculateExtreme(input, assessmentLevel)
                       : new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0],
                                                           new MacroStabilityInwardsWaternetLine[0]);
        }

        /// <summary>
        /// Gets the calculated waternet for daily circumstances.
        /// </summary>
        /// <param name="input">The input to calculate the waternet for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <returns>Returns the corresponding derived waternet value.</returns>
        public static MacroStabilityInwardsWaternet GetWaternetDaily(MacroStabilityInwardsInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return input.SoilProfileUnderSurfaceLine != null
                       ? WaternetCalculationService.CalculateDaily(input)
                       : new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0],
                                                           new MacroStabilityInwardsWaternetLine[0]);
        }
    }
}