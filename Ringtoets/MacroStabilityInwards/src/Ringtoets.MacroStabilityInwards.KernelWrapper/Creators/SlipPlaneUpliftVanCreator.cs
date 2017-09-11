// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Deltares.WTIStability;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="SlipPlaneUpliftVan"/> instances which are required by the <see cref="MacroStabilityInwardsCalculator"/>.
    /// </summary>
    internal static class SlipPlaneUpliftVanCreator
    {
        /// <summary>
        /// Creates a <see cref="SlipPlaneUpliftVan"/> based on the given <paramref name="input"/>,
        /// which can be used in the <see cref="MacroStabilityInwardsCalculator"/>.
        /// </summary>
        /// <param name="input">The <see cref="MacroStabilityInwardsCalculatorInput"/> to get the information from.</param>
        /// <returns>A new <see cref="SlipPlaneUpliftVan"/> with the given information from <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>        
        public static SlipPlaneUpliftVan Create(MacroStabilityInwardsCalculatorInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return new SlipPlaneUpliftVan
            {
                SlipPlaneLeftGrid = CreateGrid(input.LeftGrid),
                SlipPlaneRightGrid = CreateGrid(input.RightGrid),
                SlipPlaneTangentLine = CreateTangentline(input)
            };
        }

        private static SlipCircleTangentLine CreateTangentline(MacroStabilityInwardsCalculatorInput input)
        {
            return new SlipCircleTangentLine
            {
                AutomaticAtBoundaries = input.TangentLineAutomaticAtBoundaries,
                TangentLineZTop = input.TangentLineZTop,
                TangentLineZBottom = input.TangentLineZBottom,
                TangentLineNumber = 1
            };
        }

        private static SlipCircleGrid CreateGrid(MacroStabilityInwardsGrid grid)
        {
            return new SlipCircleGrid
            {
                GridXLeft = grid.XLeft,
                GridXRight = grid.XRight,
                GridZTop = grid.ZTop,
                GridZBottom = grid.ZBottom,
                GridXNumber = grid.NumberOfHorizontalPoints,
                GridZNumber = grid.NumberOfVerticalPoints
            };
        }
    }
}