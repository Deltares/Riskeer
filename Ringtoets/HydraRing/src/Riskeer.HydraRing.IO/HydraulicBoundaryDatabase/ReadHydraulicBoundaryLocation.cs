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

namespace Riskeer.HydraRing.IO.HydraulicBoundaryDatabase
{
    /// <summary>
    /// Location that is read from a hydraulic boundary database file.
    /// </summary>
    public class ReadHydraulicBoundaryLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadHydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="id">The database id of the read hydraulic boundary location.</param>
        /// <param name="name">The name of the read hydraulic boundary location.</param>
        /// <param name="coordinateX">The x coordinate of the read hydraulic boundary location.</param>
        /// <param name="coordinateY">The y coordinate of the read hydraulic boundary location.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public ReadHydraulicBoundaryLocation(long id, string name, double coordinateX, double coordinateY)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Id = id;
            Name = name;
            CoordinateX = coordinateX;
            CoordinateY = coordinateY;
        }

        /// <summary>
        /// Gets the database id of the read hydraulic boundary location.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the name of the read hydraulic boundary location.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the x coordinate of the read hydraulic boundary location.
        /// </summary>
        public double CoordinateX { get; }

        /// <summary>
        /// Gets the y coordinate of the read hydraulic boundary location.
        /// </summary>
        public double CoordinateY { get; }
    }
}