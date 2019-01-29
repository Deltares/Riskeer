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
using Ringtoets.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.IO.Configurations.Helpers
{
    /// <summary>
    /// Extension methods for converting <see cref="MacroStabilityInwardsGrid"/> to <see cref="MacroStabilityInwardsGridConfiguration"/>.
    /// </summary>
    public static class MacroStabilityInwardsGridConversionExtensions
    {
        /// <summary>
        /// Configure a new <see cref="MacroStabilityInwardsGridConfiguration"/> with 
        /// values taken from <paramref name="macroStabilityInwardsGrid"/>.
        /// </summary>
        /// <param name="macroStabilityInwardsGrid">The grid to take the values from.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsGridConfiguration"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="macroStabilityInwardsGrid"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsGridConfiguration ToMacroStabilityInwardsGridConfiguration(this MacroStabilityInwardsGrid macroStabilityInwardsGrid)
        {
            if (macroStabilityInwardsGrid == null)
            {
                throw new ArgumentNullException(nameof(macroStabilityInwardsGrid));
            }

            return new MacroStabilityInwardsGridConfiguration
            {
                XLeft = macroStabilityInwardsGrid.XLeft,
                XRight = macroStabilityInwardsGrid.XRight,
                ZTop = macroStabilityInwardsGrid.ZTop,
                ZBottom = macroStabilityInwardsGrid.ZBottom,
                NumberOfHorizontalPoints = macroStabilityInwardsGrid.NumberOfHorizontalPoints,
                NumberOfVerticalPoints = macroStabilityInwardsGrid.NumberOfVerticalPoints
            };
        }
    }
}