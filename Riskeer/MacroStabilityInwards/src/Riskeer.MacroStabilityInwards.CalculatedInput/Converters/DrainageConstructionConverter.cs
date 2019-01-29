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
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.CalculatedInput.Converters
{
    /// <summary>
    /// Converter to convert <see cref="IMacroStabilityInwardsWaternetInput"/> drainage properties
    /// into <see cref="DrainageConstruction"/>.
    /// </summary>
    public static class DrainageConstructionConverter
    {
        /// <summary>
        /// Converts <see cref="IMacroStabilityInwardsWaternetInput"/> drainage properties
        /// into <see cref="DrainageConstruction"/>.
        /// </summary>
        /// <param name="input">The input to get the properties from.</param>
        /// <returns>The converted <see cref="DrainageConstruction"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/>
        /// is <c>null</c>.</exception>
        public static DrainageConstruction Convert(IMacroStabilityInwardsWaternetInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            bool isClayDike = input.DikeSoilScenario == MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay
                              || input.DikeSoilScenario == MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand;

            return !isClayDike && input.DrainageConstructionPresent
                       ? new DrainageConstruction(input.XCoordinateDrainageConstruction, input.ZCoordinateDrainageConstruction)
                       : new DrainageConstruction();
        }
    }
}