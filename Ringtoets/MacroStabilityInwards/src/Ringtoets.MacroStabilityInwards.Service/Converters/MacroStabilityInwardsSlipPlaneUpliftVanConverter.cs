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
using Core.Common.Base.Data;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;

namespace Ringtoets.MacroStabilityInwards.Service.Converters
{
    /// <summary>
    /// Converter to convert <see cref="UpliftVanCalculationGridResult"/>
    /// into <see cref="MacroStabilityInwardsSlipPlaneUpliftVan"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSlipPlaneUpliftVanConverter
    {
        /// <summary>
        /// Converts <see cref="UpliftVanCalculationGridResult"/>
        /// into <see cref="MacroStabilityInwardsSlipPlaneUpliftVan"/>.
        /// </summary>
        /// <param name="result">The result to convert.</param>
        /// <returns>The converted <see cref="MacroStabilityInwardsSlipPlaneUpliftVan"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/>
        /// is <c>null</c>.</exception>
        public static MacroStabilityInwardsSlipPlaneUpliftVan Convert(UpliftVanCalculationGridResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            MacroStabilityInwardsGrid leftGrid = ConvertGrid(result.LeftGrid);
            MacroStabilityInwardsGrid rightGrid = ConvertGrid(result.RightGrid);

            return new MacroStabilityInwardsSlipPlaneUpliftVan(leftGrid, rightGrid, result.TangentLines);
        }

        private static MacroStabilityInwardsGrid ConvertGrid(UpliftVanGrid grid)
        {
            return new MacroStabilityInwardsGrid
            {
                XLeft = (RoundedDouble) grid.XLeft,
                XRight = (RoundedDouble) grid.XRight,
                ZTop = (RoundedDouble) grid.ZTop,
                ZBottom = (RoundedDouble) grid.ZBottom,
                NumberOfHorizontalPoints = grid.NumberOfHorizontalPoints,
                NumberOfVerticalPoints = grid.NumberOfVerticalPoints
            };
        }
    }
}