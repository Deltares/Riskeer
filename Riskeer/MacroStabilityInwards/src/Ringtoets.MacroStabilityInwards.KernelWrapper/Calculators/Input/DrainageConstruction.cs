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

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input
{
    /// <summary>
    /// A drainage construction which its properties have been adapted to perform a calculation.
    /// </summary>
    public class DrainageConstruction
    {
        /// <summary>
        /// Creates a new instance of <see cref="DrainageConstruction"/>.
        /// </summary>
        /// <remarks><see cref="IsPresent"/> is set to <c>false</c>; <see cref="XCoordinate"/> and <see cref="ZCoordinate"/>
        /// are set to <see cref="double.NaN"/>.</remarks>
        public DrainageConstruction()
        {
            IsPresent = false;
            XCoordinate = double.NaN;
            ZCoordinate = double.NaN;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DrainageConstruction"/>.
        /// </summary>
        /// <param name="xCoordinate">The x coordinate of the drainage construction.</param>
        /// <param name="zCoordinate">The z coordinate of the drainage construction.</param>
        /// <remarks><see cref="IsPresent"/> is set to <c>true</c>.</remarks>
        public DrainageConstruction(double xCoordinate, double zCoordinate)
        {
            IsPresent = true;
            XCoordinate = xCoordinate;
            ZCoordinate = zCoordinate;
        }

        /// <summary>
        /// Gets whether the drainage construction is present.
        /// </summary>
        public bool IsPresent { get; }

        /// <summary>
        /// Gets the x coordinate of the drainage construction.
        /// </summary>
        public double XCoordinate { get; }

        /// <summary>
        /// Gets the z coordinate of the drainage construction.
        /// </summary>
        public double ZCoordinate { get; }
    }
}