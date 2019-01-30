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
using Core.Common.Base.Geometry;

namespace Riskeer.Common.IO.Structures
{
    /// <summary>
    /// Representation of a structure as read from a shapefile.
    /// </summary>
    public class StructureLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="StructureLocation"/>.
        /// </summary>
        /// <param name="id">The identifier for this <see cref="StructureLocation"/>.</param>
        /// <param name="name">The name of this <see cref="StructureLocation"/>.</param>
        /// <param name="point">The coordinates of this <see cref="StructureLocation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StructureLocation(string id, string name, Point2D point)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            Id = id;
            Name = name;
            Point = point;
        }

        /// <summary>
        /// Gets the identifier for this <see cref="StructureLocation"/>.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the name of this <see cref="StructureLocation"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the actual location of this <see cref="StructureLocation"/>.
        /// </summary>
        public Point2D Point { get; }
    }
}