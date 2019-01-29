// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Collections.ObjectModel;

namespace Core.Common.Base.Geometry
{
    /// <summary>
    /// Class that captures the intersection calculation result between two <see cref="Segment2D"/>
    /// instances.
    /// </summary>
    public class Segment2DIntersectSegment2DResult
    {
        private Segment2DIntersectSegment2DResult(Intersection2DType type, Point2D[] points)
        {
            IntersectionType = type;
            IntersectionPoints = new ReadOnlyCollection<Point2D>(points);
        }

        /// <summary>
        /// Gets the type of the intersection found.
        /// </summary>
        public Intersection2DType IntersectionType { get; }

        /// <summary>
        /// Gets the intersection points, if any.
        /// </summary>
        /// <remarks>
        /// <para>If <see cref="IntersectionType"/> has a value of <see cref="Intersection2DType.Intersects"/>,
        /// the array holds the single intersection points found.</para>
        /// <para>If <see cref="IntersectionType"/> has a value of <see cref="Intersection2DType.Overlaps"/>,
        /// the array holds the two points defining the overlapping area for both segments.</para>
        /// </remarks>
        public ReadOnlyCollection<Point2D> IntersectionPoints { get; }

        /// <summary>
        /// Creates the calculation result for having found no intersections.
        /// </summary>
        public static Segment2DIntersectSegment2DResult CreateNoIntersectResult()
        {
            return new Segment2DIntersectSegment2DResult(Intersection2DType.DoesNotIntersect, new Point2D[0]);
        }

        /// <summary>
        /// Creates the calculation result for having found a single intersection.
        /// </summary>
        /// <param name="intersectionPoint">The intersection point.</param>
        public static Segment2DIntersectSegment2DResult CreateIntersectionResult(Point2D intersectionPoint)
        {
            return new Segment2DIntersectSegment2DResult(Intersection2DType.Intersects, new[]
            {
                intersectionPoint
            });
        }

        /// <summary>
        /// Creates the calculation result for having found an overlap between the two segments.
        /// </summary>
        /// <param name="start">The start of the overlapping segment.</param>
        /// <param name="end">The end of the overlapping segment.</param>
        public static Segment2DIntersectSegment2DResult CreateOverlapResult(Point2D start, Point2D end)
        {
            return new Segment2DIntersectSegment2DResult(Intersection2DType.Overlaps, new[]
            {
                start,
                end
            });
        }
    }
}