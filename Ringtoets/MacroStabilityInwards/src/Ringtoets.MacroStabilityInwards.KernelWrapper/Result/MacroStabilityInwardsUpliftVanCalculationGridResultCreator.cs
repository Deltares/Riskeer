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
using System.Linq;
using Deltares.WTIStability;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Result
{
    /// <summary>
    /// Creates <see cref="MacroStabilityInwardsUpliftVanCalculationGridResult"/> instances.
    /// </summary>
    public static class MacroStabilityInwardsUpliftVanCalculationGridResultCreator
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityInwardsUpliftVanCalculationGridResult"/> based on the information
        /// given in the <paramref name="slipPlaneUpliftVan"/>.
        /// </summary>
        /// <param name="slipPlaneUpliftVan">The output to create the result for.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsUpliftVanCalculationGridResult"/> with information
        /// taken from the <paramref name="slipPlaneUpliftVan"/></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="slipPlaneUpliftVan"/>
        /// is <c>null</c>.</exception>
        public static MacroStabilityInwardsUpliftVanCalculationGridResult Create(SlipPlaneUpliftVan slipPlaneUpliftVan)
        {
            if (slipPlaneUpliftVan == null)
            {
                throw new ArgumentNullException(nameof(slipPlaneUpliftVan));
            }

            MacroStabilityInwardsGridResult leftGrid = CreateGrid(slipPlaneUpliftVan.SlipPlaneLeftGrid);
            MacroStabilityInwardsGridResult rightGrid = CreateGrid(slipPlaneUpliftVan.SlipPlaneRightGrid);

            return new MacroStabilityInwardsUpliftVanCalculationGridResult(leftGrid, rightGrid, slipPlaneUpliftVan.SlipPlaneTangentLine.BoundaryHeights
                                                                                                                  .Select(tl => tl.Height));
        }

        private static MacroStabilityInwardsGridResult CreateGrid(SlipCircleGrid grid)
        {
            return new MacroStabilityInwardsGridResult(grid.GridXLeft, grid.GridXRight, grid.GridZTop, grid.GridZBottom, grid.GridXNumber, grid.GridZNumber);
        }
    }
}