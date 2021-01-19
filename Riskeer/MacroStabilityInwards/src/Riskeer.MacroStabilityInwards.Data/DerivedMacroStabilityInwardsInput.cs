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
using Core.Common.Base.Data;
using Riskeer.MacroStabilityInwards.CalculatedInput;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data
{
    /// <summary>
    /// Class responsible for calculating the derived macro stability inwards input.
    /// </summary>
    public static class DerivedMacroStabilityInwardsInput
    {
        /// <summary>
        /// Gets the calculated Waternet for extreme circumstances.
        /// </summary>
        /// <param name="input">The input to calculate the Waternet for.</param>
        /// <param name="generalInput">General calculation parameters that are the same across all calculations.</param>
        /// <param name="assessmentLevel">The assessment level at stake.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <returns>Returns the corresponding derived Waternet value.</returns>
        public static MacroStabilityInwardsWaternet GetWaternetExtreme(MacroStabilityInwardsInput input,
                                                                       GeneralMacroStabilityInwardsInput generalInput,
                                                                       RoundedDouble assessmentLevel)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            return input.SoilProfileUnderSurfaceLine != null
                       ? WaternetCalculationService.CalculateExtreme(input, generalInput, assessmentLevel)
                       : new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0],
                                                           new MacroStabilityInwardsWaternetLine[0]);
        }

        /// <summary>
        /// Gets the calculated Waternet for daily circumstances.
        /// </summary>
        /// <param name="input">The input to calculate the Waternet for.</param>
        /// <param name="generalInput">General calculation parameters that are the same across all calculations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <returns>Returns the corresponding derived Waternet value.</returns>
        public static MacroStabilityInwardsWaternet GetWaternetDaily(MacroStabilityInwardsInput input,
                                                                     GeneralMacroStabilityInwardsInput generalInput)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            return input.SoilProfileUnderSurfaceLine != null
                       ? WaternetCalculationService.CalculateDaily(input, generalInput)
                       : new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0],
                                                           new MacroStabilityInwardsWaternetLine[0]);
        }
    }
}