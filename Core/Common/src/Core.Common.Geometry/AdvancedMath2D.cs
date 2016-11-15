﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using GeoAPI.Geometries;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NetTopologySuite.Geometries;

using Point2D = Core.Common.Base.Geometry.Point2D;

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
                return BuildSeparateAreasFromCoordinateList(polygonA.Intersection(polygonB));
            }
            catch (TopologyException e)
            {
                throw new InvalidPolygonException(e.Message, e);
            }
        }

        /// <summary>
        /// Transforms X coordinates in a 2D X, Y plane using:
        /// - A reference point as starting point of the line.
        /// - An offset at which the reference coincides with the X axis.
        /// - A rotation from North of the X coordinates around the origin after substracting the offset in degrees.
        /// </summary>
        /// <param name="xCoordinates">The X coordinates of a line.</param>
        /// <param name="referencePoint">The point of reference where the line is transposed to.</param>
        /// <param name="offset">The offset at which the referencePoints coincides with the X axis.</param>
        /// <param name="rotation">The rotation from the North in degrees.</param>
        /// <returns>A collection of <see cref="Point2D"/> with the transformed X coordinates.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="xCoordinates"/> or
        /// <paramref name="referencePoint"/> is <c>null</c>.</exception>
        public static Point2D[] FromXToXY(double[] xCoordinates, Point2D referencePoint, double offset, double rotation)
        {
            if (xCoordinates == null)
            {
                throw new ArgumentNullException("xCoordinates", "Cannot transform to coordinates without a source.");
            }
            if (referencePoint == null)
            {
                throw new ArgumentNullException("referencePoint", "Cannot transform to coordinates without a reference point.");
            }

            return xCoordinates.Select(coordinate =>
            {
                Vector2D referenceVector = new Vector2D(referencePoint.X, referencePoint.Y);
                Vector2D pointVector = referenceVector + new Vector2D(0, coordinate - offset).Rotate(-rotation, AngleUnit.Degrees);
                return new Point2D(pointVector.X, pointVector.Y);
            }).ToArray();
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

        private static IEnumerable<Point2D[]> BuildSeparateAreasFromCoordinateList(IGeometry geometry)
        {
            var geometryCollection = geometry as GeometryCollection;
            if (geometryCollection == null)
            {
                if (geometry.Coordinates.Any())
                {
                    return new[]
                    {
                        geometry.Coordinates.Distinct().Select(c => new Point2D(c.X, c.Y)).ToArray()
                    };
                }
                return Enumerable.Empty<Point2D[]>();
            }

            var areas = new List<Point2D[]>();
            if (!geometryCollection.IsEmpty)
            {
                for (int i = 0; i < geometry.NumGeometries; i++)
                {
                    areas = areas.Union(BuildSeparateAreasFromCoordinateList(geometryCollection[i])).ToList();
                }
            }
            return areas;
        }
    }
}