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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace Core.Common.Geometry
{
    /// <summary>
    /// This class contains more advanced mathematical routines for 2D geometries.
    /// </summary>
    public static class AdvancedMath2D
    {
        /// <summary>
        /// Calculates the intersection between two polygons, which can result in any number of polygons which represent the intersecting area. Polygons
        /// are defined by an array of points.
        /// </summary>
        /// <param name="pointsOfPolygonA">Points of the first polygon.</param>
        /// <param name="pointsOfPolygonB">Points of the second polygon.</param>
        /// <returns>A collection of point arrays. Each point array describes an intersecting area of the polygons.</returns>
        /// <exception cref="InvalidPolygonException"></exception>
        public static IEnumerable<Point2D[]> PolygonIntersectionWithPolygon(IEnumerable<Point2D> pointsOfPolygonA, IEnumerable<Point2D> pointsOfPolygonB)
        {
            Polygon polygonA = PointsToPolygon(pointsOfPolygonA);
            Polygon polygonB = PointsToPolygon(pointsOfPolygonB);

            try
            {
                IGeometry intersection = polygonA.Intersection(polygonB);
                return BuildSeparateAreasFromCoordinateList(intersection.Coordinates);
            }
            catch (TopologyException e)
            {
                throw new InvalidPolygonException(e.Message, e);
            }
        }

        private static Polygon PointsToPolygon(IEnumerable<Point2D> points)
        {
            var pointList = points.ToList();
            var firstPoint = pointList.First();

            if (!firstPoint.Equals(pointList.Last()))
            {
                pointList.Add(firstPoint);
            }
            var coordinates = pointList.Select(p => new Coordinate(p.X, p.Y)).ToArray();

            return new Polygon(new LinearRing(coordinates));
        }

        private static IEnumerable<Point2D[]> BuildSeparateAreasFromCoordinateList(Coordinate[] coordinates)
        {
            var areas = new List<Point2D[]>();
            HashSet<Point2D> area = new HashSet<Point2D>();

            foreach (var coordinate in coordinates)
            {
                var added = area.Add(new Point2D(coordinate.X, coordinate.Y));
                if (!added)
                {
                    areas.Add(area.ToArray());
                    area = new HashSet<Point2D>();
                }
            }
            return areas;
        }
    }
}