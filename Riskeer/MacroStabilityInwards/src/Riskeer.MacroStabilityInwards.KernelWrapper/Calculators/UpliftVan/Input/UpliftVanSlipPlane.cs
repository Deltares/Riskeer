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

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input
{
    /// <summary>
    /// The Uplift Van slip plane that is used to perform a calculation.
    /// </summary>
    public class UpliftVanSlipPlane
    {
        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanSlipPlane"/>.
        /// </summary>
        /// <remarks>The following values are set:
        /// <list type="bullet">
        /// <item><see cref="GridAutomaticDetermined"/> is set to <c>true</c>;</item>
        /// <item><see cref="LeftGrid"/> is set to <c>null</c>;</item>
        /// <item><see cref="RightGrid"/> is set to <c>null</c>;</item>
        /// <item><see cref="TangentLinesAutomaticAtBoundaries"/> is set to <c>true</c>;</item>
        /// <item><see cref="TangentZTop"/> is set to <see cref="double.NaN"/>;</item>
        /// <item><see cref="TangentZBottom"/> is set to <see cref="double.NaN"/>;</item>
        /// <item><see cref="TangentLineNumber"/> is set to <c>0</c>.</item>
        /// </list>
        /// </remarks>
        public UpliftVanSlipPlane() : this(double.NaN, double.NaN, 0)
        {
            GridAutomaticDetermined = true;
            TangentLinesAutomaticAtBoundaries = true;
            GridNumberOfRefinements = 1;
        }

        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanSlipPlane"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="leftGrid"/>
        /// or <paramref name="rightGrid"/> is <c>null</c>.</exception>
        /// <remarks>The following values are set:
        /// <list type="bullet">
        /// <item><see cref="GridAutomaticDetermined"/> is set to <c>false</c>;</item>
        /// <item><see cref="TangentLinesAutomaticAtBoundaries"/> is set to <c>true</c>;</item>
        /// <item><see cref="TangentZTop"/> is set to <see cref="double.NaN"/>;</item>
        /// <item><see cref="TangentZBottom"/> is set to <see cref="double.NaN"/>;</item>
        /// <item><see cref="TangentLineNumber"/> is set to <c>0</c>.</item>
        /// </list>
        /// </remarks>
        public UpliftVanSlipPlane(UpliftVanGrid leftGrid, UpliftVanGrid rightGrid)
            : this(double.NaN, double.NaN, 0)
        {
            SetGrids(leftGrid, rightGrid);
            TangentLinesAutomaticAtBoundaries = true;
            GridNumberOfRefinements = 0;
        }

        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanSlipPlane"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="leftGrid"/>
        /// or <paramref name="rightGrid"/> is <c>null</c>.</exception>
        /// <remarks>The following values are set:
        /// <list type="bullet">
        /// <item><see cref="GridAutomaticDetermined"/> is set to <c>false</c>;</item>
        /// <item><see cref="TangentLinesAutomaticAtBoundaries"/> is set to <c>false</c>.</item>
        /// </list>
        /// </remarks>
        public UpliftVanSlipPlane(UpliftVanGrid leftGrid, UpliftVanGrid rightGrid, double tangentZTop,
                                  double tangentZBottom, int tangentLineNumber)
            : this(tangentZTop, tangentZBottom, tangentLineNumber)
        {
            SetGrids(leftGrid, rightGrid);
            GridNumberOfRefinements = 0;
        }

        private UpliftVanSlipPlane(double tangentZTop, double tangentZBottom, int tangentLineNumber)
        {
            TangentZTop = tangentZTop;
            TangentZBottom = tangentZBottom;
            TangentLineNumber = tangentLineNumber;
            TangentLineNumberOfRefinements = 4;
        }

        /// <summary>
        /// Gets whether the grid is automatic determined.
        /// </summary>
        public bool GridAutomaticDetermined { get; }

        /// <summary>
        /// Gets the left grid.
        /// </summary>
        public UpliftVanGrid LeftGrid { get; private set; }

        /// <summary>
        /// Gets the right grid.
        /// </summary>
        public UpliftVanGrid RightGrid { get; private set; }

        /// <summary>
        /// Gets the number of grid refinements.
        /// </summary>
        public int GridNumberOfRefinements { get; }

        /// <summary>
        /// Gets whether the tangent line boundaries should be determined automatically.
        /// </summary>
        public bool TangentLinesAutomaticAtBoundaries { get; }

        /// <summary>
        /// Gets the tangent line top boundary.
        /// [m+NAP]
        /// </summary>
        public double TangentZTop { get; }

        /// <summary>
        /// Gets the tangent line bottom boundary.
        /// [m+NAP]
        /// </summary>
        public double TangentZBottom { get; }

        /// <summary>
        /// Gets the number of tangent lines.
        /// </summary>
        public int TangentLineNumber { get; }

        /// <summary>
        /// Gets the number of tangent line refinements.
        /// </summary>
        public int TangentLineNumberOfRefinements { get; }

        private void SetGrids(UpliftVanGrid leftGrid, UpliftVanGrid rightGrid)
        {
            if (leftGrid == null)
            {
                throw new ArgumentNullException(nameof(leftGrid));
            }

            if (rightGrid == null)
            {
                throw new ArgumentNullException(nameof(rightGrid));
            }

            LeftGrid = leftGrid;
            RightGrid = rightGrid;
        }
    }
}