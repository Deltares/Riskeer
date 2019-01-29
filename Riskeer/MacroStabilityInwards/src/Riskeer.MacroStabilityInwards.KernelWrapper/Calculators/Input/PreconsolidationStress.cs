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

using System;
using Core.Common.Base.Geometry;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input
{
    /// <summary>
    /// A preconsolidation stress for which its properties have been adapted to perform a calculation.
    /// </summary>
    public class PreconsolidationStress
    {
        /// <summary>
        /// Creates a new instance of <see cref="PreconsolidationStress"/>.
        /// </summary>
        /// <param name="coordinate">The coordinate of the stress.</param>
        /// <param name="stress">The value of the stress.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="coordinate"/>
        /// is <c>null</c>.</exception>
        public PreconsolidationStress(Point2D coordinate, double stress)
        {
            if (coordinate == null)
            {
                throw new ArgumentNullException(nameof(coordinate));
            }

            Coordinate = coordinate;
            Stress = stress;
        }

        /// <summary>
        /// Gets the coordinate of the stress.
        /// </summary>
        public Point2D Coordinate { get; }

        /// <summary>
        /// Gets the value of the stress.
        /// </summary>
        public double Stress { get; }
    }
}