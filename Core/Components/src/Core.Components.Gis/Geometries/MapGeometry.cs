// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
    /// The geometry containing the points defined in the RD-new coordinate system.
    /// </summary>
    public class MapGeometry
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapGeometry"/>.
        /// </summary>
        /// <param name="pointCollections">A sequence of <see cref="Point2D"/>-sequences
        /// that define the geometry in the RD-new coordinate system.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pointCollections"/> is <c>null</c>.</exception>
        public MapGeometry(IEnumerable<IEnumerable<Point2D>> pointCollections)
        {
            if (pointCollections == null)
            {
                throw new ArgumentNullException(nameof(pointCollections), @"MapGeometry cannot be created without points.");
            }

            PointCollections = pointCollections;
        }

        /// <summary>
        /// Gets the points associated with the <see cref="MapGeometry"/> defined in the
        /// RD-new coordinate system.
        /// </summary>
        public IEnumerable<IEnumerable<Point2D>> PointCollections { get; }
    }
}