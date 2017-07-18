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
    /// Definition of a surfaceline for piping.
    /// </summary>
    public class SurfaceLine : Observable
    {
        private const int numberOfDecimalPlaces = 2;

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
        /// <exception cref="ArgumentException">Thrown when any element of <paramref name="points"/> is <c>null</c>.</exception>
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
            Points = points.Select(p => new Point3D(p)).ToArray();
        }

        /// <summary>
        /// Projects the points in <see cref="Points"/> to localized coordinate (LZ-plane) system.
        /// Z-values are retained, and the first point is put a L=0.
        /// </summary>
        /// <returns>Collection of 2D points in the LZ-plane.</returns>
        private RoundedPoint2DCollection ProjectGeometryToLZ()
        {
            int count = Points.Length;
            if (count == 0)
            {
                return new RoundedPoint2DCollection(numberOfDecimalPlaces, Enumerable.Empty<Point2D>());
            }

            Point3D first = Points.First();
            if (count == 1)
            {
                return new RoundedPoint2DCollection(numberOfDecimalPlaces, new[]
                {
                    new Point2D(0.0, first.Z)
                });
            }

            Point3D last = Points.Last();
            var firstPoint = new Point2D(first.X, first.Y);
            var lastPoint = new Point2D(last.X, last.Y);
            return new RoundedPoint2DCollection(numberOfDecimalPlaces, Points.Select(p => p.ProjectIntoLocalCoordinates(firstPoint, lastPoint)));
        }

        /// <summary>
        /// Checks whether the current <see cref="Points"/> collection results in 
        /// <see cref="SurfaceLine"/> of zero length.
        /// </summary>
        /// <returns><c>true</c> if the surface line has a length of zero; <c>false</c>
        /// otherwise.</returns>
        public bool IsZeroLength()
        {
            Point3D lastPoint = null;
            foreach (Point3D point in Points)
            {
                if (lastPoint != null)
                {
                    if (!Equals(lastPoint, point))
                    {
                        return false;
                    }
                }
                lastPoint = point;
            }
            return true;
        }

        /// <summary>
        /// Checks whether the current <see cref="ProjectGeometryToLZ()"/> would
        /// return a reclining geometry. That is, given a point from the geometry
        /// with an L-coordinate, has a point further in the geometry that has an
        /// L-coordinate smaller than the L-coordinate of the given point.
        /// </summary>
        /// <returns><c>true</c> if the surface line  is reclining; <c>false</c>
        /// otherwise.</returns>
        public bool IsReclining()
        {
            double[] lCoordinates = ProjectGeometryToLZ().Select(p => p.X).ToArray();
            for (var i = 1; i < lCoordinates.Length; i++)
            {
                if (lCoordinates[i - 1] > lCoordinates[i])
                {
                    return true;
                }
            }
            return false;
        }
    }
}