// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;

namespace Core.Components.Gis.Geometries
{
    /// <summary>
    /// The geometry containing the points.
    /// </summary>
    public class MapGeometry
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapGeometry"/>.
        /// </summary>
        /// <param name="points">An <see cref="IEnumerable{T}"/> of <see cref="Point2D"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public MapGeometry(IEnumerable<Point2D> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points", "MapGeometry can't be created without points.");
            }
            Points = points;
        }

        /// <summary>
        /// Gets the points associated with the <see cref="MapGeometry"/>.
        /// </summary>
        public IEnumerable<Point2D> Points { get; private set; }
    }
}
