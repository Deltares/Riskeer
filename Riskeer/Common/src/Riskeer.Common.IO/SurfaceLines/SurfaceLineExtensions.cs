// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.Properties;

namespace Riskeer.Common.IO.SurfaceLines
{
    /// <summary>
    /// Extension methods for the <see cref="SurfaceLine"/>.
    /// </summary>
    public static class SurfaceLineExtensions
    {
        /// <summary>
        /// The type of the intersection possible with a reference line.
        /// </summary>
        public enum ReferenceLineIntersectionsResult
        {
            NoIntersections,
            OneIntersection,
            MultipleIntersectionsOrOverlap
        }

        /// <summary>
        /// Finds out if there is an intersection of the <paramref name="surfaceLine"/> and
        /// the <paramref name="referenceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line.</param>
        /// <param name="referenceLine">The reference line.</param>
        /// <returns>A new <see cref="ReferenceLineIntersectionResult"/> with a type of intersection and
        /// possibly an intersection point, if there was only one found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when the surface line:
        /// <list type="bullet">
        /// <item>has no intersections with the reference line;</item>
        /// <item>has more than one intersection with the reference line.</item>
        /// </list>
        /// </exception>
        public static Point2D GetSingleReferenceLineIntersection(this SurfaceLine surfaceLine, ReferenceLine referenceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }

            ReferenceLineIntersectionResult result = GetReferenceLineIntersections(referenceLine, surfaceLine);

            if (result.TypeOfIntersection == ReferenceLineIntersectionsResult.NoIntersections)
            {
                string message = string.Format(Resources.SurfaceLineExtensions_GetSingleReferenceLineIntersection_SurfaceLine_0_does_not_correspond_to_current_referenceline_Could_be_caused_coordinates_being_local_coordinate_system,
                                               surfaceLine.Name);
                throw new ImportedDataTransformException(message);
            }

            if (result.TypeOfIntersection == ReferenceLineIntersectionsResult.MultipleIntersectionsOrOverlap)
            {
                string message = string.Format(Resources.SurfaceLineExtensions_GetSingleReferenceLineIntersection_SurfaceLine_0_does_not_correspond_to_current_referenceline, surfaceLine.Name);
                throw new ImportedDataTransformException(message);
            }

            return result.IntersectionPoint;
        }

        private static ReferenceLineIntersectionResult GetReferenceLineIntersections(ReferenceLine referenceLine, SurfaceLine surfaceLine)
        {
            IEnumerable<Segment2D> surfaceLineSegments = Math2D.ConvertPointsToLineSegments(surfaceLine.Points.Select(p => new Point2D(p.X, p.Y))).ToArray();
            IEnumerable<Segment2D> referenceLineSegments = Math2D.ConvertPointsToLineSegments(referenceLine.Points).ToArray();

            return GetReferenceLineIntersectionsResult(surfaceLineSegments, referenceLineSegments);
        }

        private static ReferenceLineIntersectionResult GetReferenceLineIntersectionsResult(IEnumerable<Segment2D> surfaceLineSegments,
                                                                                           IEnumerable<Segment2D> referenceLineSegments)
        {
            Point2D intersectionPoint = null;
            foreach (Segment2D surfaceLineSegment in surfaceLineSegments)
            {
                foreach (Segment2D referenceLineSegment in referenceLineSegments)
                {
                    Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(surfaceLineSegment, referenceLineSegment);

                    if (result.IntersectionType == Intersection2DType.Intersects)
                    {
                        Point2D resultIntersectionPoint = result.IntersectionPoints[0];
                        if (intersectionPoint != null && !intersectionPoint.Equals(resultIntersectionPoint))
                        {
                            // Early exit as multiple intersections is a return result:
                            return ReferenceLineIntersectionResult.CreateMultipleIntersectionsOrOverlapResult();
                        }

                        intersectionPoint = resultIntersectionPoint;
                    }

                    if (result.IntersectionType == Intersection2DType.Overlaps)
                    {
                        // Early exit as overlap is a return result:
                        return ReferenceLineIntersectionResult.CreateMultipleIntersectionsOrOverlapResult();
                    }
                }
            }

            return intersectionPoint != null
                       ? ReferenceLineIntersectionResult.CreateIntersectionResult(intersectionPoint)
                       : ReferenceLineIntersectionResult.CreateNoSingleIntersectionResult();
        }

        /// <summary>
        /// Result of finding the intersections of a surface line and a reference line.
        /// </summary>
        public class ReferenceLineIntersectionResult
        {
            private ReferenceLineIntersectionResult(ReferenceLineIntersectionsResult typeOfIntersection, Point2D intersectionPoint)
            {
                TypeOfIntersection = typeOfIntersection;
                IntersectionPoint = intersectionPoint;
            }

            /// <summary>
            /// Gets the type of intersection that was found.
            /// </summary>
            public ReferenceLineIntersectionsResult TypeOfIntersection { get; }

            /// <summary>
            /// Gets the intersection point of the surface line and the reference line if 
            /// there was only one found; or <c>null</c> otherwise.
            /// </summary>
            public Point2D IntersectionPoint { get; }

            public static ReferenceLineIntersectionResult CreateNoSingleIntersectionResult()
            {
                return new ReferenceLineIntersectionResult(ReferenceLineIntersectionsResult.NoIntersections, null);
            }

            public static ReferenceLineIntersectionResult CreateIntersectionResult(Point2D point)
            {
                return new ReferenceLineIntersectionResult(ReferenceLineIntersectionsResult.OneIntersection, point);
            }

            public static ReferenceLineIntersectionResult CreateMultipleIntersectionsOrOverlapResult()
            {
                return new ReferenceLineIntersectionResult(ReferenceLineIntersectionsResult.MultipleIntersectionsOrOverlap, null);
            }
        }
    }
}