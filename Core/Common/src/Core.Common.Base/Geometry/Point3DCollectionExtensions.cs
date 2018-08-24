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
using System.Linq;

namespace Core.Common.Base.Geometry
{
    /// <summary>
    /// Extension methods for collections of <see cref="Point3D"/>.
    /// </summary>
    public static class Point3DCollectionExtensions
    {
        /// <summary>
        /// Projects the points in <paramref name="points"/> to localized coordinate (LZ-plane) system.
        /// Z-values are retained, and the first point is put a L=0.
        /// </summary>
        /// <param name="points">Points to project.</param>
        /// <returns>Collection of 2D points in the LZ-plane.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<Point2D> ProjectToLZ(this IEnumerable<Point3D> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }
            int count = points.Count();
            if (count == 0)
            {
                return Enumerable.Empty<Point2D>();
            }

            Point3D first = points.First();
            if (count == 1)
            {
                return new[]
                {
                    new Point2D(0.0, first.Z)
                };
            }

            Point3D last = points.Last();
            var firstPoint = new Point2D(first.X, first.Y);
            var lastPoint = new Point2D(last.X, last.Y);
            return points.Select(p => p.ProjectIntoLocalCoordinates(firstPoint, lastPoint)).ToArray();
        }

        /// <summary>
        /// Checks whether the <paramref name="points"/> collection results in 
        /// a line of zero length.
        /// </summary>
        /// <param name="points">The points forming a line to check.</param>
        /// <returns><c>true</c> if the line has a length of zero; <c>false</c>
        /// otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/>
        /// is <c>null</c>.</exception>
        public static bool IsZeroLength(this IEnumerable<Point3D> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }
            Point3D lastPoint = null;
            foreach (Point3D point in points)
            {
                if (lastPoint != null && !Equals(lastPoint, point))
                {
                    return false;
                }

                lastPoint = point;
            }
            return true;
        }

        /// <summary>
        /// Checks whether the locally projected <paramref name="points"/> would
        /// return a reclining geometry. That is, given a point from the geometry
        /// with an L-coordinate, has a point further in the geometry that has an
        /// L-coordinate smaller than the L-coordinate of the given point.
        /// </summary>
        /// <param name="points">The points forming a line to check.</param>
        /// <returns><c>true</c> if the localized (in 2D) line is reclining;
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/>
        /// is <c>null</c>.</exception>
        public static bool IsReclining(this IEnumerable<Point3D> points)
        {
            return points.ProjectToLZ().IsReclining();
        }
    }
}