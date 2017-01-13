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
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using DotSpatialReproject = DotSpatial.Projections.Reproject;

namespace Core.Components.DotSpatial.Layer.BruTile.Projections
{
    /// <summary>
    /// Extension methods for reprojecting objects from one coordinate system to another.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Reprojection/ReprojectExtensions.cs
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
        public static ILinearRing Reproject(this ILinearRing ring, ProjectionInfo source, ProjectionInfo target)
        {
            var seq = Reproject(ring.Coordinates.Densify(36), source, target);
            return ring.Factory.CreateLinearRing(seq);
        }

        /// <summary>
        /// Reprojects a <see cref="Extent"/>.
        /// </summary>
        /// <param name="extent">The object to be reprojected.</param>
        /// <param name="source">The coordinate system corresponding to <paramref name="ring"/>.</param>
        /// <param name="target">The target coordinate system.</param>
        /// <returns>The reprojected <see cref="Extent"/>.</returns>
        public static Extent Reproject(this Extent extent, ProjectionInfo source, ProjectionInfo target)
        {
            if (target.Transform == null)
            {
                return extent;
            }

            double[] xy = ToSequence(extent);
            DotSpatialReproject.ReprojectPoints(xy, null, source, target, 0, xy.Length/2);
            return ToExtent(xy);
        }

        private static IList<Coordinate> Reproject(this IList<Coordinate> coordinates, ProjectionInfo source, ProjectionInfo target)
        {
            if (target.Transform == null)
            {
                return coordinates;
            }

            var ords = new double[coordinates.Count*2];
            var z = new double[coordinates.Count];
            var j = 0;
            for (var i = 0; i < coordinates.Count; i++)
            {
                Coordinate c = coordinates[i];
                ords[j++] = c.X;
                ords[j++] = c.Y;
                z[i] = double.IsNaN(c.Z) ? 0 : c.Z;
            }

            DotSpatialReproject.ReprojectPoints(ords, z, source, target, 0, coordinates.Count);

            var lst = new List<Coordinate>(coordinates.Count);
            j = 0;
            for (var i = 0; i < coordinates.Count; i++)
            {
                lst.Add(new Coordinate(ords[j++], ords[j++]));
            }
            return lst;
        }

        /// <summary>
        /// Returns a collection of <see cref="Coordinate"/> that has the same shape as
        /// the target, but with more points set 'edge' (effectively increasing its resolution).
        /// </summary>
        /// <param name="self">The original collection of <see cref="Coordinate"/>.</param>
        /// <param name="intervals">The number of segments each 'edge' should be subdivided in.</param>
        /// <returns></returns>
        /// <remarks>A value of 4 of <paramref name="intervals"/> means that 3 equally spaced
        /// points are introduced along the line between two <see cref="Coordinate"/> instances
        /// in <see cref="self"/>.</remarks>
        private static IList<Coordinate> Densify(this IList<Coordinate> self, int intervals)
        {
            if (self.Count < 2)
            {
                return self;
            }

            var res = new List<Coordinate>(intervals*(self.Count - 1) + 1);
            Coordinate start = self[0];

            for (var i = 1; i < self.Count; i++)
            {
                res.Add(start);
                Coordinate end = self[i];

                double dx = (end.X - start.X)/intervals;
                double dy = (end.Y - start.Y)/intervals;

                for (var j = 0; j < intervals - 1; j++)
                {
                    start = new Coordinate(start.X + dx, start.Y + dy);
                    res.Add(start);
                }
                res.Add(end);
                start = end;
            }
            return res;
        }

        /// <summary>
        /// Transforms the coordinates of an <see cref="Extent"/> to a sequence of xy pairs.
        /// </summary>
        /// <param name="extent">The extent.</param>
        /// <returns>The xy sequence.</returns>
        /// <remarks>The resolution of the edges of <paramref name="extent"/> are increased
        /// as a straight edge in the source coordinate system can lead to a curved edge in
        /// the target coordinate system.</remarks>
        private static double[] ToSequence(Extent extent)
        {
            const int horizontal = 72;
            const int vertical = 36;
            var result = new double[horizontal*vertical*2];

            double dx = extent.Width/(horizontal - 1);
            double dy = extent.Height/(vertical - 1);

            double minY = extent.MinY;
            var k = 0;
            for (var i = 0; i < vertical; i++)
            {
                double minX = extent.MinX;
                for (var j = 0; j < horizontal; j++)
                {
                    result[k++] = minX;
                    result[k++] = minY;
                    minX += dx;
                }
                minY += dy;
            }

            return result;
        }

        private static Extent ToExtent(double[] xyOrdinates)
        {
            double minX = double.MaxValue, maxX = double.MinValue;
            double minY = double.MaxValue, maxY = double.MinValue;

            var i = 0;
            while (i < xyOrdinates.Length)
            {
                double xyOrdinate = xyOrdinates[i];
                if (!double.IsNaN(xyOrdinate) &&
                    (double.MinValue < xyOrdinate && xyOrdinate < double.MaxValue))
                {
                    if (minX > xyOrdinate)
                    {
                        minX = xyOrdinate;
                    }
                    if (maxX < xyOrdinate)
                    {
                        maxX = xyOrdinate;
                    }
                }
                i += 1;
                if (!double.IsNaN(xyOrdinate) &&
                    (double.MinValue < xyOrdinate && xyOrdinate < double.MaxValue))
                {
                    if (minY > xyOrdinate)
                    {
                        minY = xyOrdinate;
                    }
                    if (maxY < xyOrdinate)
                    {
                        maxY = xyOrdinate;
                    }
                }
                i += 1;
            }
            return new Extent(minX, minY, maxX, maxY);
        }
    }
}