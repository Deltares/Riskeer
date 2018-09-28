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

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan
{
    /// <summary>
    /// The grid of an Uplift Van calculation.
    /// </summary>
    public class UpliftVanGrid
    {
        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanGrid"/>.
        /// </summary>
        /// <param name="xLeft">The left boundary of the grid.</param>
        /// <param name="xRight">The right boundary of the grid.</param>
        /// <param name="zTop">The top boundary of the grid.</param>
        /// <param name="zBottom">The bottom boundary of the grid.</param>
        /// <param name="numberOfHorizontalPoints">The number of horizontal points.</param>
        /// <param name="numberOfVerticalPoints">The number of vertical points.</param>
        public UpliftVanGrid(double xLeft, double xRight, double zTop, double zBottom, int numberOfHorizontalPoints, int numberOfVerticalPoints)
        {
            XLeft = xLeft;
            XRight = xRight;
            ZTop = zTop;
            ZBottom = zBottom;
            NumberOfHorizontalPoints = numberOfHorizontalPoints;
            NumberOfVerticalPoints = numberOfVerticalPoints;
        }

        /// <summary>
        /// Gets the left boundary.
        /// [m]
        /// </summary>
        public double XLeft { get; }

        /// <summary>
        /// Gets the right boundary.
        /// [m]
        /// </summary>
        public double XRight { get; }

        /// <summary>
        /// Gets the top boundary.
        /// [m+NAP]
        /// </summary>
        public double ZTop { get; }

        /// <summary>
        /// Gets the bottom boundary.
        /// [m+NAP]
        /// </summary>
        public double ZBottom { get; }

        /// <summary>
        /// Gets the number of horizontal points.
        /// </summary>
        public int NumberOfHorizontalPoints { get; }

        /// <summary>
        /// Gets the number of vertical points.
        /// </summary>
        public int NumberOfVerticalPoints { get; }
    }
}