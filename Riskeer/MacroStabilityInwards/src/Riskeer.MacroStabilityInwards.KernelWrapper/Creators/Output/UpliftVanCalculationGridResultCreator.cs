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
using Deltares.MacroStability.CSharpWrapper;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Output
{
    /// <summary>
    /// Creates <see cref="UpliftVanCalculationGridResult"/> instances.
    /// </summary>
    internal static class UpliftVanCalculationGridResultCreator
    {
        /// <summary>
        /// Creates a <see cref="UpliftVanCalculationGridResult"/> based on the information
        /// given in the <paramref name="upliftVanCalculationGrid"/>.
        /// </summary>
        /// <param name="upliftVanCalculationGrid">The output to create the result for.</param>
        /// <returns>A new <see cref="UpliftVanCalculationGridResult"/> with information
        /// taken from the <paramref name="upliftVanCalculationGrid"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="upliftVanCalculationGrid"/>
        /// is <c>null</c>.</exception>
        public static UpliftVanCalculationGridResult Create(UpliftVanCalculationGrid upliftVanCalculationGrid)
        {
            if (upliftVanCalculationGrid == null)
            {
                throw new ArgumentNullException(nameof(upliftVanCalculationGrid));
            }

            UpliftVanGrid leftGrid = CreateGrid(upliftVanCalculationGrid.LeftGrid);
            UpliftVanGrid rightGrid = CreateGrid(upliftVanCalculationGrid.RightGrid);

            return new UpliftVanCalculationGridResult(leftGrid, rightGrid, upliftVanCalculationGrid.TangentLines);
        }

        private static UpliftVanGrid CreateGrid(CalculationGrid grid)
        {
            return new UpliftVanGrid(grid.GridXLeft, grid.GridXRight, grid.GridZTop, grid.GridZBottom, grid.GridXNumber, grid.GridZNumber);
        }
    }
}