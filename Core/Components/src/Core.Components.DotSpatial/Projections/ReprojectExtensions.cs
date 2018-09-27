// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using DotSpatialReproject = DotSpatial.Projections.Reproject;

namespace Core.Components.DotSpatial.Projections
{
    /// <summary>
    /// Extension methods for reprojecting objects from one coordinate system to another.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Reprojection/ReprojectExtensions.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
    /// </remarks>
    internal static class ReprojectExtensions
    {
        /// <summary>
        /// Reprojects a <see cref="ILinearRing"/>.
        /// </summary>
        /// <param name="ring">The object to be reprojected.</param>
        /// <param name="source">The coordinate system corresponding to <paramref name="ring"/>.</param>
        /// <param name="target">The target coordinate system.</param>
        /// <returns>The reprojected <see cref="ILinearRing"/>.</returns>
        /// <remarks>The returned object is specified in a higher resolution than <paramref name="ring"/>.
        /// This is done as a straight edge in <paramref name="source"/> can lead to a curved
        /// edge in <paramref name="target"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="ring"/> has less
        /// than 3 coordinates.</exception>
        public static ILinearRing Reproject(this ILinearRing ring, ProjectionInfo source, ProjectionInfo target)
        {
            if (ring == null)
            {
                throw new ArgumentNullException(nameof(ring));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (ring.Coordinates.Count < 3)
            {
                throw new ArgumentException(@"Ring must contain at least 3 coordinates.",
                                            nameof(ring));
            }

            IList<Coordinate> seq = Reproject(ring.Coordinates.Densify(36), source, target);
            return ring.Factory.CreateLinearRing(seq);
        }

        /// <summary>
        /// Reprojects a <see cref="Extent"/>.
        /// </summary>
        /// <param name="extent">The object to be reprojected.</param>
        /// <param name="source">The coordinate system corresponding to <paramref name="extent"/>.</param>
        /// <param name="target">The target coordinate system.</param>
        /// <returns>The reprojected <see cref="Extent"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public static Extent Reproject(this Extent extent, ProjectionInfo source, ProjectionInfo target)
        {
            if (extent == null)
            {
                throw new ArgumentNullException(nameof(extent));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (target.Transform == null)
            {
                return extent;
            }

            double[] xy = ToSequence(extent);
            DotSpatialReproject.ReprojectPoints(xy, null, source, target, 0, xy.Length / 2);
            return ToExtent(xy);
        }

        private static IList<Coordinate> Reproject(this IList<Coordinate> coordinates, ProjectionInfo source, ProjectionInfo target)
        {
            if (target.Transform == null)
            {
                return coordinates;
            }

            var xy = new double[coordinates.Count * 2];
            var z = new double[coordinates.Count];
            var j = 0;
            for (var i = 0; i < coordinates.Count; i++)
            {
                Coordinate c = coordinates[i];
                xy[j] = c.X;
                xy[j + 1] = c.Y;
                j = j + 2;
                z[i] = double.IsNaN(c.Z) ? 0 : c.Z;
            }

            DotSpatialReproject.ReprojectPoints(xy, z, source, target, 0, coordinates.Count);

            var result = new List<Coordinate>(coordinates.Count);
            j = 0;
            for (var i = 0; i < coordinates.Count; i++)
            {
                result.Add(new Coordinate(xy[j++], xy[j++]));
            }

            return result;
        }

        /// <summary>
        /// Returns a collection of <see cref="Coordinate"/> that has the same shape as
        /// the target, but with more points set 'edge' (effectively increasing its resolution).
        /// </summary>
        /// <param name="original">The original collection of <see cref="Coordinate"/>.</param>
        /// <param name="intervals">The number of segments each 'edge' should be subdivided in.</param>
        /// <returns>Return the densified version based on <paramref name="original"/>.</returns>
        /// <remarks>A value of 4 of <paramref name="intervals"/> means that 3 equally spaced
        /// points are introduced along the line between two <see cref="Coordinate"/> instances
        /// in <paramref name="original"/>.</remarks>
        private static IList<Coordinate> Densify(this IList<Coordinate> original, int intervals)
        {
            return GetDensifiedCoordinates(original, intervals - 1);
        }

        private static IList<Coordinate> GetDensifiedCoordinates(IList<Coordinate> original, int numberOfAdditionalPoints)
        {
            int numberOfEdges = original.Count - 1;
            var resultList = new List<Coordinate>(numberOfEdges * (numberOfAdditionalPoints + 1) + 1)
            {
                original[0]
            };
            for (var i = 1; i <= numberOfEdges; i++)
            {
                resultList.AddRange(GetEdgePointsExcludingStart(original[i - 1], original[i], numberOfAdditionalPoints));
            }

            return resultList;
        }

        private static IEnumerable<Coordinate> GetEdgePointsExcludingStart(Coordinate start, Coordinate end, int numberOfAdditionalPoints)
        {
            if (numberOfAdditionalPoints < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfAdditionalPoints),
                                                      @"Number of additional points cannot be negative.");
            }

            double dx = (end.X - start.X) / (numberOfAdditionalPoints + 1);
            double dy = (end.Y - start.Y) / (numberOfAdditionalPoints + 1);

            for (var i = 1; i <= numberOfAdditionalPoints; i++)
            {
                yield return new Coordinate(start.X + i * dx, start.Y + i * dy);
            }

            yield return end;
        }

        private static double[] ToSequence(Extent extent)
        {
            const int horizontalResolution = 72;
            const int verticalResolution = 36;
            var xy = new double[horizontalResolution * verticalResolution * 2];

            double dx = extent.Width / (horizontalResolution - 1);
            double dy = extent.Height / (verticalResolution - 1);

            // Define a lattice of points, because there exist coordinate transformations
            // that place original center-points to edge-point in the target coordinate system:
            double y = extent.MinY;
            var k = 0;
            for (var i = 0; i < verticalResolution; i++)
            {
                double x = extent.MinX;
                for (var j = 0; j < horizontalResolution; j++)
                {
                    xy[k] = x;
                    xy[k + 1] = y;
                    k = k + 2;
                    x += dx;
                }

                y += dy;
            }

            return xy;
        }

        private static Extent ToExtent(double[] xyOrdinates)
        {
            double minX = double.MaxValue, maxX = double.MinValue;
            double minY = double.MaxValue, maxY = double.MinValue;

            var i = 0;
            while (i < xyOrdinates.Length)
            {
                double x = xyOrdinates[i];
                if (!double.IsNaN(x) && double.MinValue < x && x < double.MaxValue)
                {
                    minX = Math.Min(minX, x);
                    maxX = Math.Max(maxX, x);
                }

                i += 1;

                double y = xyOrdinates[i];
                if (!double.IsNaN(y) && double.MinValue < y && y < double.MaxValue)
                {
                    minY = Math.Min(minY, y);
                    maxY = Math.Max(maxY, y);
                }

                i += 1;
            }

            return new Extent(minX, minY, maxX, maxY);
        }
    }
}