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

using Deltares.WTIStability;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil
{
    /// <summary>
    /// Factory to create simple <see cref="SlipPlaneUpliftVan"/>
    /// instances that can be used for testing.
    /// </summary>
    public static class SlipPlaneUpliftVanTestFactory
    {
        /// <summary>
        /// Creates a simple <see cref="SlipPlaneUpliftVan"/>.
        /// </summary>
        /// <returns>A simple <see cref="SlipPlaneUpliftVan"/> with default values.</returns>
        public static SlipPlaneUpliftVan Create()
        {
            return new SlipPlaneUpliftVan
            {
                SlipPlaneLeftGrid = new SlipCircleGrid
                {
                    GridXLeft = 0.1,
                    GridXRight = 0.2,
                    GridZTop = 0.3,
                    GridZBottom = 0.4,
                    GridXNumber = 1,
                    GridZNumber = 2
                },
                SlipPlaneRightGrid = new SlipCircleGrid
                {
                    GridXLeft = 0.5,
                    GridXRight = 0.6,
                    GridZTop = 0.7,
                    GridZBottom = 0.8,
                    GridXNumber = 3,
                    GridZNumber = 4
                },
                SlipPlaneTangentLine = new SlipCircleTangentLine
                {
                    BoundaryHeights =
                    {
                        new TangentLine(1.1),
                        new TangentLine(2.2)
                    }
                }
            };
        }
    }
}