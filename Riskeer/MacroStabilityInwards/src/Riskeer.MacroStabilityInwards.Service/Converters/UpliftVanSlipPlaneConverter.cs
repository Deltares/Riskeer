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
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;

namespace Riskeer.MacroStabilityInwards.Service.Converters
{
    /// <summary>
    /// Converter to convert <see cref="MacroStabilityInwardsInput"/> slip plane properties
    /// into <see cref="UpliftVanSlipPlane"/>.
    /// </summary>
    internal static class UpliftVanSlipPlaneConverter
    {
        /// <summary>
        /// Converts <see cref="MacroStabilityInwardsInput"/> slip plane properties
        /// into <see cref="UpliftVanSlipPlane"/>.
        /// </summary>
        /// <param name="input">The input to get the properties from.</param>
        /// <returns>The converted <see cref="UpliftVanSlipPlane"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/>
        /// is <c>null</c>.</exception>
        public static UpliftVanSlipPlane Convert(MacroStabilityInwardsInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.GridDeterminationType == MacroStabilityInwardsGridDeterminationType.Automatic)
            {
                return new UpliftVanSlipPlane();
            }

            UpliftVanGrid leftGrid = ConvertGrid(input.LeftGrid);
            UpliftVanGrid rightGrid = ConvertGrid(input.RightGrid);

            return input.TangentLineDeterminationType == MacroStabilityInwardsTangentLineDeterminationType.Specified
                       ? new UpliftVanSlipPlane(leftGrid, rightGrid, input.TangentLineZTop, input.TangentLineZBottom, input.TangentLineNumber)
                       : new UpliftVanSlipPlane(leftGrid, rightGrid);
        }

        private static UpliftVanGrid ConvertGrid(MacroStabilityInwardsGrid grid)
        {
            return new UpliftVanGrid(grid.XLeft, grid.XRight, grid.ZTop, grid.ZBottom, grid.NumberOfHorizontalPoints, grid.NumberOfVerticalPoints);
        }
    }
}