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

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input
{
    /// <summary>
    /// The Uplift Van slip plane constraints that are used to perform a calculation.
    /// </summary>
    public class UpliftVanSlipPlaneConstraints
    {
        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanSlipPlaneConstraints"/>.
        /// </summary>
        /// <remarks>The following values are set:
        /// <list type="bullet">
        /// <item><see cref="CreateZones"/> is set to <c>false</c>;</item>
        /// <item><see cref="AutomaticForbiddenZones"/> is set to <c>false</c>;</item>
        /// <item><see cref="ZoneBoundaryLeft"/> is set to <see cref="double.NaN"/>;</item>
        /// <item><see cref="ZoneBoundaryRight"/> is set to <see cref="double.NaN"/>;</item>
        /// <item><see cref="SlipPlaneMinimumLength"/> is set to <see cref="double.NaN"/>;</item>
        /// <item><see cref="SlipPlaneMinimumDepth"/> is set to <see cref="double.NaN"/>.</item>
        /// </list>
        /// </remarks>
        public UpliftVanSlipPlaneConstraints()
        {
            CreateZones = false;
            AutomaticForbiddenZones = false;
            ZoneBoundaryLeft = double.NaN;
            ZoneBoundaryRight = double.NaN;
            SlipPlaneMinimumLength = double.NaN;
            SlipPlaneMinimumDepth = double.NaN;
        }

        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanSlipPlaneConstraints"/>.
        /// </summary>
        /// <param name="createZones">Indicator whether zones should be created.</param>
        /// <param name="automaticForbiddenZones">Indicator whether forbidden zones
        /// should be automatically determined.</param>
        /// <param name="zoneBoundaryLeft">The left zone boundary.</param>
        /// <param name="zoneBoundaryRight">The right zone boundary.</param>
        /// <param name="slipPlaneMinimumDepth">The slip plane minimum depth.</param>
        /// <param name="slipPlaneMinimumLength">The slip plane minimum length.</param>
        public UpliftVanSlipPlaneConstraints(bool createZones, bool automaticForbiddenZones, double zoneBoundaryLeft,
                                             double zoneBoundaryRight, double slipPlaneMinimumDepth, double slipPlaneMinimumLength)
        {
            CreateZones = createZones;
            AutomaticForbiddenZones = automaticForbiddenZones;
            ZoneBoundaryLeft = zoneBoundaryLeft;
            ZoneBoundaryRight = zoneBoundaryRight;
            SlipPlaneMinimumDepth = slipPlaneMinimumDepth;
            SlipPlaneMinimumLength = slipPlaneMinimumLength;
        }

        /// <summary>
        /// Gets whether zones should be created.
        /// </summary>
        public bool CreateZones { get; }

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
    }
}