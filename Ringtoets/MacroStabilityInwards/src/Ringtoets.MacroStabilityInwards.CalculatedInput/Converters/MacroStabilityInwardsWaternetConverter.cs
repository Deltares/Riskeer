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
using System.Collections.Generic;
using System.Linq;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.CalculatedInput.Converters
{
    /// <summary>
    /// Converter to convert <see cref="WaternetCalculatorResult"/>
    /// into <see cref="MacroStabilityInwardsWaternet"/>.
    /// </summary>
    internal static class MacroStabilityInwardsWaternetConverter
    {
        /// <summary>
        /// Converts <see cref="WaternetCalculatorResult"/> into <see cref="MacroStabilityInwardsWaternet"/>.
        /// </summary>
        /// <param name="calculatorResult">The result to convert.</param>
        /// <returns>The converted <see cref="MacroStabilityInwardsWaternet"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculatorResult"/>
        /// is <c>null</c>.</exception>
        public static MacroStabilityInwardsWaternet Convert(WaternetCalculatorResult calculatorResult)
        {
            if (calculatorResult == null)
            {
                throw new ArgumentNullException(nameof(calculatorResult));
            }

            IDictionary<WaternetPhreaticLineResult, MacroStabilityInwardsPhreaticLine> phreaticLineLookup = calculatorResult.PhreaticLines
                                                                                                                            .ToDictionary(pl => pl, ConvertPhreaticLine);

            return new MacroStabilityInwardsWaternet(phreaticLineLookup.Values.ToArray(),
                                                     calculatorResult.WaternetLines
                                                                     .Select(wl => ConvertWaternetLine(wl, phreaticLineLookup)).ToArray());
        }

        private static MacroStabilityInwardsWaternetLine ConvertWaternetLine(WaternetLineResult waternetLine,
                                                                             IDictionary<WaternetPhreaticLineResult, MacroStabilityInwardsPhreaticLine> phreaticLines)
        {
            return new MacroStabilityInwardsWaternetLine(waternetLine.Name,
                                                         waternetLine.Geometry,
                                                         phreaticLines[waternetLine.PhreaticLine]);
        }

        private static MacroStabilityInwardsPhreaticLine ConvertPhreaticLine(WaternetPhreaticLineResult phreaticLine)
        {
            return new MacroStabilityInwardsPhreaticLine(phreaticLine.Name, phreaticLine.Geometry);
        }
    }
}