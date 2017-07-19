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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.SurfaceLines
{
    /// <summary>
    /// Definition of the surface line, which is the top level geometry of a dike.
    /// </summary>
    public class SurfaceLine : Observable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceLine"/> class.
        /// </summary>
        public SurfaceLine()
        {
            Name = string.Empty;
            Points = new Point3D[0];
        }

        /// <summary>
        /// Gets or sets the name of the surfaceline.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the 3D points describing its geometry.
        /// </summary>
        public Point3D[] Points { get; private set; }

        /// <summary>
        /// Sets the geometry of the surfaceline.
        /// </summary>
        /// <param name="points">The collection of points defining the surfaceline geometry.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when either:
        /// <list type="bullet">
        /// <item>Any element of <paramref name="points"/> is <c>null</c></item>
        /// <item>The given points are too close to eachother</item>
        /// <item>The given points form a reclining line: one in which the local L-coordinates of the points
        /// are not in ascending order.</item>
        /// </list></exception>
        public void SetGeometry(IEnumerable<Point3D> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points), Resources.SurfaceLine_Collection_of_points_for_geometry_is_null);
            }
            if (points.Any(p => p == null))
            {
                throw new ArgumentException(Resources.SurfaceLine_A_point_in_the_collection_was_null);
            }
            if (points.IsZeroLength())
            {
                throw new ArgumentException(Resources.SurfaceLinesCsvReader_ReadLine_SurfaceLine_has_zero_length);
            }
            if (points.IsReclining())
            {
                throw new ArgumentException(Resources.SurfaceLinesCsvReader_ReadLine_SurfaceLine_has_reclining_geometry);
            }
            Points = points.Select(p => new Point3D(p)).ToArray();
        }
    }
}