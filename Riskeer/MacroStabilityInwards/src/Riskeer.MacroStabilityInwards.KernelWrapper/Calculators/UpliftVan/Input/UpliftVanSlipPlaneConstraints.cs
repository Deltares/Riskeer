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

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input
{
    /// <summary>
    /// The Uplift Van slip plane constraints that are used to perform a calculation.
    /// </summary>
    public class UpliftVanSlipPlaneConstraints
    {
        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanSlipPlaneConstraints"/>.
        /// </summary>
        /// <param name="slipPlaneMinimumDepth">The slip plane minimum depth.</param>
        /// <param name="slipPlaneMinimumLength">The slip plane minimum length.</param>
        /// <remarks>The following values are set:
        /// <list type="bullet">
        /// <item><see cref="AutomaticForbiddenZones"/> is set to <c>true</c>;</item>
        /// <item><see cref="ZoneBoundaryLeft"/> is set to <see cref="double.NaN"/>;</item>
        /// <item><see cref="ZoneBoundaryRight"/> is set to <see cref="double.NaN"/>.</item>
        /// </list>
        /// </remarks>
        public UpliftVanSlipPlaneConstraints(double slipPlaneMinimumDepth,
                                             double slipPlaneMinimumLength)
            : this(slipPlaneMinimumDepth, slipPlaneMinimumLength,
                   double.NaN, double.NaN, true) {}

        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanSlipPlaneConstraints"/>.
        /// </summary>
        /// <param name="slipPlaneMinimumDepth">The slip plane minimum depth.</param>
        /// <param name="slipPlaneMinimumLength">The slip plane minimum length.</param>
        /// <param name="zoneBoundaryLeft">The left zone boundary.</param>
        /// <param name="zoneBoundaryRight">The right zone boundary.</param>
        /// <remarks><see cref="AutomaticForbiddenZones"/> is set to <c>false</c>.</remarks>
        public UpliftVanSlipPlaneConstraints(double slipPlaneMinimumDepth, double slipPlaneMinimumLength,
                                             double zoneBoundaryLeft, double zoneBoundaryRight)
            : this(slipPlaneMinimumDepth, slipPlaneMinimumLength, zoneBoundaryLeft,
                   zoneBoundaryRight, false) {}

        private UpliftVanSlipPlaneConstraints(double slipPlaneMinimumDepth, double slipPlaneMinimumLength,
                                              double zoneBoundaryLeft, double zoneBoundaryRight, bool automaticForbiddenZones)
        {
            AutomaticForbiddenZones = automaticForbiddenZones;
            ZoneBoundaryLeft = zoneBoundaryLeft;
            ZoneBoundaryRight = zoneBoundaryRight;
            SlipPlaneMinimumDepth = slipPlaneMinimumDepth;
            SlipPlaneMinimumLength = slipPlaneMinimumLength;
            AllowLeftToRight = true;
            AllowRightToLeft = false;
            AllowSwapLeftRight = false;
        }

        /// <summary>
        /// Gets whether forbidden zones are automatically determined or not.
        /// </summary>
        public bool AutomaticForbiddenZones { get; }

        /// <summary>
        /// Gets the left zone boundary.
        /// [m]
        /// </summary>
        public double ZoneBoundaryLeft { get; }

        /// <summary>
        /// Gets the right zone boundary.
        /// [m]
        /// </summary>
        public double ZoneBoundaryRight { get; }

        /// <summary>
        /// Gets the minimum depth of the slip plane.
        /// [m]
        /// </summary>
        public double SlipPlaneMinimumDepth { get; }

        /// <summary>
        /// Gets the minimum length of the slip plane.
        /// [m]
        /// </summary>
        public double SlipPlaneMinimumLength { get; }

        /// <summary>
        /// Gets whether the slip plane constraints are allowed from left to right.
        /// </summary>
        public bool AllowLeftToRight { get; }

        /// <summary>
        /// Gets whether the slip plane constraints are allowed from right to left.
        /// </summary>
        public bool AllowRightToLeft { get; }

        /// <summary>
        /// Gets whether the slip plane constraints are to be swapped.
        /// </summary>
        public bool AllowSwapLeftRight { get; }
    }
}