﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Service.Converters
{
    /// <summary>
    /// Converter to convert <see cref="MacroStabilityInwardsInput"/> drainage properties
    /// into <see cref="UpliftVanDrainageConstruction"/>.
    /// </summary>
    internal static class UpliftVanDrainageConstructionConverter
    {
        /// <summary>
        /// Converts <see cref="MacroStabilityInwardsInput"/> drainage properties
        /// into <see cref="UpliftVanDrainageConstruction"/>.
        /// </summary>
        /// <param name="input">The input to get the properties from.</param>
        /// <returns>The converted <see cref="UpliftVanDrainageConstruction"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/>
        /// is <c>null</c>.</exception>
        public static UpliftVanDrainageConstruction Convert(MacroStabilityInwardsInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            bool isClayDike = input.DikeSoilScenario == MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay
                              || input.DikeSoilScenario == MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand;

            return !isClayDike && input.DrainageConstructionPresent
                       ? new UpliftVanDrainageConstruction(input.XCoordinateDrainageConstruction, input.ZCoordinateDrainageConstruction)
                       : new UpliftVanDrainageConstruction();
        }
    }
}