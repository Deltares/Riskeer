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
using Deltares.MacroStability.Data;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="SlipPlaneUpliftVan"/> instances which are required by <see cref="IUpliftVanKernel"/>.
    /// </summary>
    internal static class SlipPlaneUpliftVanCreator
    {
        /// <summary>
        /// Creates a <see cref="SlipPlaneUpliftVan"/> based on the given <paramref name="slipPlane"/>,
        /// which can be used by <see cref="IUpliftVanKernel"/>.
        /// </summary>
        /// <param name="slipPlane">The <see cref="UpliftVanSlipPlane"/> to get the information from.</param>
        /// <returns>A new <see cref="SlipPlaneUpliftVan"/> with the given information from <paramref name="slipPlane"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="slipPlane"/> is <c>null</c>.</exception>
        public static SlipPlaneUpliftVan Create(UpliftVanSlipPlane slipPlane)
        {
            if (slipPlane == null)
            {
                throw new ArgumentNullException(nameof(slipPlane));
            }

            var kernelSlipPlane = new SlipPlaneUpliftVan
            {
                SlipPlaneLeftGrid = CreateGrid(slipPlane, slipPlane.LeftGrid),
                SlipPlaneRightGrid = CreateGrid(slipPlane, slipPlane.RightGrid),
                SlipPlaneTangentLine = CreateTangentLine(slipPlane)
            };

            return kernelSlipPlane;
        }

        private static SlipCircleGrid CreateGrid(UpliftVanSlipPlane slipPlane, UpliftVanGrid grid)
        {
            var slipCircleGrid = new SlipCircleGrid
            {
                NumberOfRefinements = slipPlane.GridNumberOfRefinements
            };

            if (!slipPlane.GridAutomaticDetermined)
            {
                slipCircleGrid.GridXLeft = grid.XLeft;
                slipCircleGrid.GridXRight = grid.XRight;
                slipCircleGrid.GridZTop = grid.ZTop;
                slipCircleGrid.GridZBottom = grid.ZBottom;
                slipCircleGrid.GridXNumber = grid.NumberOfHorizontalPoints;
                slipCircleGrid.GridZNumber = grid.NumberOfVerticalPoints;
            }

            return slipCircleGrid;
        }

        private static SlipCircleTangentLine CreateTangentLine(UpliftVanSlipPlane slipPlane)
        {
            var tangentLine = new SlipCircleTangentLine
            {
                NumberOfRefinements = slipPlane.TangentLineNumberOfRefinements
            };

            if (!slipPlane.TangentLinesAutomaticAtBoundaries)
            {
                tangentLine.TangentLineZTop = slipPlane.TangentZTop;
                tangentLine.TangentLineZBottom = slipPlane.TangentZBottom;
                tangentLine.TangentLineNumber = slipPlane.TangentLineNumber;
            }

            return tangentLine;
        }
    }
}