// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.HydraRing.IO.HydraulicBoundaryDatabaseContext
{
    /// <summary>
    /// Location of an hydraulic boundary in the hydraulic boundary database.
    /// </summary>
    public class HrdLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="HrdLocation"/>.
        /// </summary>
        /// <param name="hrdLocationId">HrdLocationId of the <see cref="HrdLocation"/>.</param>
        /// <param name="name">Name of the <see cref="HrdLocation"/>.</param>
        /// <param name="coordinateX">X-coordinate of the <see cref="HrdLocation"/>.</param>
        /// <param name="coordinateY">Y-coordinate of the <see cref="HrdLocation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public HrdLocation(long hrdLocationId, string name, double coordinateX, double coordinateY)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            HrdLocationId = hrdLocationId;
            Name = name;
            LocationX = coordinateX;
            LocationY = coordinateY;
        }

        /// <summary>
        /// Gets the database HrdLocationId of <see cref="HrdLocation"/>.
        /// </summary>
        public long HrdLocationId { get; private set; }

        /// <summary>
        /// Gets the name of <see cref="HrdLocation"/>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the X-coordinate of <see cref="HrdLocation"/>.
        /// </summary>
        public double LocationX { get; private set; }

        /// <summary>
        /// Gets the Y-coordinate of <see cref="HrdLocation"/>.
        /// </summary>
        public double LocationY { get; private set; }
    }
}