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
using Deltares.WTIStability;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input
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
                ActiveSide = ActiveSideType.Left
            };

            if (!slipPlane.GridAutomaticDetermined)
            {
                kernelSlipPlane.SlipPlaneLeftGrid = CreateGrid(slipPlane.LeftGrid);
                kernelSlipPlane.SlipPlaneRightGrid = CreateGrid(slipPlane.RightGrid);
            }

            kernelSlipPlane.SlipPlaneTangentLine = CreateTangentline(slipPlane);

            return kernelSlipPlane;
        }

        private static SlipCircleTangentLine CreateTangentline(UpliftVanSlipPlane slipPlane)
        {
            var tangentLine = new SlipCircleTangentLine
            {
                AutomaticAtBoundaries = slipPlane.TangentLinesAutomaticAtBoundaries
            };

            if (!slipPlane.TangentLinesAutomaticAtBoundaries)
            {
                tangentLine.TangentLineZTop = slipPlane.TangentZTop;
                tangentLine.TangentLineZBottom = slipPlane.TangentZBottom;
                tangentLine.TangentLineNumber = slipPlane.TangentLineNumber;
            }

            return tangentLine;
        }

        private static SlipCircleGrid CreateGrid(UpliftVanGrid grid)
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