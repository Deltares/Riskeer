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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Properties;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Core.Common.Base.Geometry
{
    /// <summary>
    /// This class contains general mathematical routines for 2D lines.
    /// </summary>
    public static class Math2D
    {
        /// <summary>
        /// Constant which is used to precision errors in <see cref="double"/> comparisons.
        /// </summary>
        private const double epsilonForComparisons = 1e-6;

        /// <summary>
        /// Splits the line geometry at given lengths.
        /// </summary>
        /// <param name="linePoints">The line to split.</param>
        /// <param name="lengths">The lengths where the splits should be placed.</param>
        /// <returns>A sequence of line geometries of N elements long where N is the number
        /// of elements in <paramref name="lengths"/>.</returns>
        /// <exception cref="ArgumentException">When the sum of all elements in <paramref name="lengths"/>
        /// does not fully cover the line given by <paramref name="linePoints"/> - or - when
        /// <paramref name="lengths"/> contains negative values - or - when <paramref name="linePoints"/>
        /// does not have 2 or more elements.</exception>
        public static Point2D[][] SplitLineAtLengths(IEnumerable<Point2D> linePoints, double[] lengths)
        {
            if (lengths.Any(l => l < 0))
            {
                throw new ArgumentException(Resources.Math2D_SplitLineAtLengths_All_lengths_cannot_be_negative, "lengths");
            }
            if (linePoints.Count() <= 1)
            {
                throw new ArgumentException(Resources.Math2D_SplitLineAtLengths_Not_enough_points_to_make_line, "linePoints");
            }
            Segment2D[] lineSegments = ConvertLinePointsToLineSegments(linePoints).ToArray();

            if (Math.Abs(lengths.Sum(l => l) - lineSegments.Sum(s => s.Length)) > epsilonForComparisons)
            {
                throw new ArgumentException(Resources.Math2D_SplitLineAtLengths_Sum_of_lengths_must_equal_line_length, "lengths");
            }

            return SplitLineSegmentsAtLengths(lineSegments, lengths);
        }

        /// <summary>
        /// Creates an enumerator that converts a sequence of line points to a sequence of line segments.
        /// </summary>
        /// <param name="linePoints">The line points.</param>
        /// <returns>A sequence of N elements, where N is the number of elements in <paramref name="linePoints"/>
        /// - 1, or 0 if <paramref name="linePoints"/> only has one or no elements.</returns>
        public static IEnumerable<Segment2D> ConvertLinePointsToLineSegments(IEnumerable<Point2D> linePoints)
        {
            Point2D endPoint = null;
            foreach (Point2D linePoint in linePoints)
            {
                Point2D startPoint = endPoint;
                endPoint = linePoint;

                if (startPoint != null)
                {
                    yield return new Segment2D(startPoint, endPoint);
                }
            }
        }

        /// <summary>
        /// Determines the intersection point of a line which passes through the <paramref name="line1Point1"/> and 
        /// the <paramref name="line1Point2"/>; and a line which passes through the <paramref name="line2Point1"/>
        /// and the <paramref name="line2Point2"/>.
        /// </summary>
        /// <param name="line1Point1">A <see cref="Point2D"/> which the first line passes through.</param>
        /// <param name="line1Point2">Another <see cref="Point2D"/> which the first line passes through.</param>
        /// <param name="line2Point1">A <see cref="Point2D"/> which the second line passes through.</param>
        /// <param name="line2Point2">Another <see cref="Point2D"/> which the second line passes through.</param>
        /// <returns>An <see cref="Point2D"/> with coordinates at the point where the lines intersect. Or <c>null</c> when no
        /// intersection point exists (lines are parallel).</returns>
        /// <remarks>
        /// <para>Taken from: https://www.topcoder.com/community/data-science/data-science-tutorials/geometry-concepts-line-intersection-and-its-applications/ </para>
        /// <para>Based on https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection </para>
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when <paramref name="line1Point1"/> equals <paramref name="line1Point2"/> or 
        /// <paramref name="line2Point1"/> equals <paramref name="line2Point2"/>, which makes it impossible to determine
        /// a line through the points.</exception>
        public static Point2D LineIntersectionWithLine(Point2D line1Point1, Point2D line1Point2, Point2D line2Point1, Point2D line2Point2)
        {
            if (line1Point1.Equals(line1Point2) || line2Point1.Equals(line2Point2))
            {
                throw new ArgumentException(Resources.Math2D_LineIntersectionWithLine_Line_points_are_equal);
            }

            var aLine = line1Point2.Y - line1Point1.Y;
            var bLine = line1Point1.X - line1Point2.X;
            var cLine = aLine * line1Point1.X + bLine * line1Point1.Y;

            var aOtherLine = line2Point2.Y - line2Point1.Y;
            var bOtherLine = line2Point1.X - line2Point2.X;
            var cOtherLine = aOtherLine * line2Point1.X + bOtherLine * line2Point1.Y;

            var determinant = aLine * bOtherLine - aOtherLine * bLine;
            if (Math.Abs(determinant) < epsilonForComparisons)
            {
                return null;
            }

            var x = (bOtherLine * cLine - bLine * cOtherLine) / determinant;
            var y = (aLine * cOtherLine - aOtherLine * cLine) / determinant;
            return new Point2D(x, y);
        }

        /// <summary>
        /// Determines the intersection points between multiple segments.
        /// </summary>
        /// <param name="segments">A <see cref="List{T}"/> of <see cref="Segment2D"/>.</param>
        /// <param name="segmentsToCompare">Another <see cref="List{T}"/> of <see cref="Segment2D"/> which may or may not intersect with the <see cref="Segment2D"/> from <paramref name="segments"/>.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="Point2D"/> intersection points.</returns>
        public static IEnumerable<Point2D> SegmentsIntersectionsWithSegments(IEnumerable<Segment2D> segments, IEnumerable<Segment2D> segmentsToCompare)
        {
            return segments.SelectMany(segment => segmentsToCompare, SingleSegmentIntersectionWithSingleSegment).Where(intersection => intersection != null).Distinct().ToList();
        }

        /// <summary>
        /// Determines the intersection points between two <see cref="Segment2D"/> segments.
        /// </summary>
        /// <param name="segment1">A <see cref="Segment2D"/> segment.</param>
        /// <param name="segment2">Another <see cref="Segment2D"/> segment which may or may not intersect with the <see cref="Segment2D"/> from <paramref name="segment1"/>.</param>
        /// <returns>A <see cref="Point2D"/> intersection point, or <c>null</c> when there is no intersection.</returns>
        public static Point2D SingleSegmentIntersectionWithSingleSegment(Segment2D segment1, Segment2D segment2)
        {
            if (AreEqualPoints(segment1.FirstPoint, segment1.SecondPoint))
            {
                return segment1.FirstPoint;
            }
            if (AreEqualPoints(segment1.SecondPoint, segment2.FirstPoint))
            {
                return segment1.SecondPoint;
            }

            double intersectionPoint;
            var intersects = Intersects(segment1.FirstPoint, segment1.SecondPoint, segment2.FirstPoint, segment2.SecondPoint, out intersectionPoint);

            if (!intersects)
            {
                return null;
            }

            return new Point2D
                (
                segment1.FirstPoint.X + intersectionPoint*(segment1.SecondPoint.X - segment1.FirstPoint.X),
                segment1.FirstPoint.Y + intersectionPoint*(segment1.SecondPoint.Y - segment1.FirstPoint.Y)
                );
        }

        /// <summary>
        /// Determines if two <see cref="Point2D"/> points are equal.
        /// </summary>
        /// <param name="point1">The first <see cref="Point2D"/> point.</param>
        /// <param name="point2">The second <see cref="Point2D"/> point.</param>
        /// <returns><c>True</c> when the points are equal. <c>False</c> otherwise.</returns>
        public static bool AreEqualPoints(Point2D point1, Point2D point2)
        {
            return Math.Abs(point1.X - point2.X) < epsilonForComparisons && Math.Abs(point1.Y - point2.Y) < epsilonForComparisons;
        }

        /// <summary>
        /// Determines the intersection points of a <see cref="IEnumerable{T}"/> of <see cref="Segment2D"/> with a vertical line
        /// which is plotted at x=<paramref name="verticalLineX"/>.
        /// </summary>
        /// <param name="segments">A collection of segments that possibly intersect with the
        /// vertical line at x=<paramref name="verticalLineX"/>.</param>
        /// <param name="verticalLineX">The x coordinate of the vertical line.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Point2D"/> with all intersection points of the 
        /// <paramref name="segments"/> with the vertical line at x=<paramref name="verticalLineX"/>.</returns>
        /// <remark>Segments which have length=0 or which are vertical, will not return an intersection point.</remark>
        public static IEnumerable<Point2D> SegmentsIntersectionWithVerticalLine(IEnumerable<Segment2D> segments, double verticalLineX)
        {
            var intersectionPointY = new Collection<Point2D>();

            foreach (Segment2D segment in segments.Where(s => s.ContainsX(verticalLineX)))
            {
                Point2D intersectionPoint = LineIntersectionWithVerticalLine(segment.FirstPoint, segment.SecondPoint, verticalLineX);

                if (intersectionPoint != null)
                {
                    intersectionPointY.Add(intersectionPoint);
                }
            }

            return intersectionPointY;
        }

        /// <summary>
        /// Creates an enumerator that converts a sequence of <see cref="Point3D"/> line points to a sequence of line segments.
        /// </summary>
        /// <param name="linePoints">The <see cref="Point3D"/> points to convert.</param>
        /// <returns>A sequence of N elements, where N is the number of elements in <paramref name="linePoints"/>
        /// - 1, or 0 if <paramref name="linePoints"/> only has one or no elements.</returns>
        public static IEnumerable<Segment2D> Convert3DPointsToLineSegments(IEnumerable<Point3D> linePoints)
        {
            var points = new Collection<Point2D>();

            foreach (var point in linePoints)
            {
                points.Add(Convert3DPointTo2DPoint(point));
            }

            return ConvertLinePointsToLineSegments(points);
        }

        private static Point2D Convert3DPointTo2DPoint(Point3D point)
        {
            return new Point2D(point.X, point.Y);
        }

        private static bool Intersects(Point2D point1, Point2D point2, Point2D point3, Point2D point4, out double result)
        {
            var aLine = (point1.Y - point3.Y)*(point4.X - point3.X) - (point1.X - point3.X)*(point4.Y - point3.Y);
            var bLine = (point2.X - point1.X)*(point4.Y - point3.Y) - (point2.Y - point1.Y)*(point4.X - point3.X);

            if (Math.Abs(bLine) < epsilonForComparisons) // parallel lines so no intersection anywhere in space (in curved space, maybe, but not here in Euclidian space.)
            {
                result = 0;
                return false;
            }

            result = aLine/bLine;

            aLine = (point1.Y - point3.Y)*(point2.X - point1.X) - (point1.X - point3.X)*(point2.Y - point1.Y);
            var cLine = aLine/bLine;

            return ((result >= 0 && result <= 1) && cLine >= 0) && cLine <= 1;
        }

        private static Point2D[][] SplitLineSegmentsAtLengths(Segment2D[] lineSegments, double[] lengths)
        {
            var splitResults = new Point2D[lengths.Length][];

            int index = 0;
            double lineSegmentRemainder = lineSegments[index].Length;
            double distanceOnSegment = 0;
            Point2D startPoint = lineSegments[index].FirstPoint;
            for (int i = 0; i < lengths.Length; i++)
            {
                double splitDistanceRemainder = lengths[i];
                var subLine = new List<Point2D>
                {
                    startPoint
                };

                while (splitDistanceRemainder > lineSegmentRemainder)
                {
                    splitDistanceRemainder -= lineSegmentRemainder;
                    subLine.Add(lineSegments[index].SecondPoint);

                    if (index < lineSegments.Length - 1)
                    {
                        lineSegmentRemainder = lineSegments[++index].Length;
                        distanceOnSegment = 0;
                    }
                }

                if (i < lengths.Length - 1)
                {
                    Point2D interpolatedPoint = GetInterpolatedPoint(lineSegments[index], distanceOnSegment + splitDistanceRemainder);
                    subLine.Add(interpolatedPoint);

                    distanceOnSegment += splitDistanceRemainder;
                    lineSegmentRemainder -= splitDistanceRemainder;
                    startPoint = interpolatedPoint;
                }
                else
                {
                    EnsureLastSplitResultHasEndPointOfLine(subLine, lineSegments);
                }

                splitResults[i] = subLine.ToArray();
            }
            return splitResults;
        }

        private static void EnsureLastSplitResultHasEndPointOfLine(ICollection<Point2D> subLine, IList<Segment2D> lineSegments)
        {
            var lastSegmentIndex = lineSegments.Count - 1;
            if (!subLine.Contains(lineSegments[lastSegmentIndex].SecondPoint))
            {
                subLine.Add(lineSegments[lastSegmentIndex].SecondPoint);
            }
        }

        private static Point2D GetInterpolatedPoint(Segment2D lineSegment, double splitDistance)
        {
            var interpolationFactor = splitDistance / lineSegment.Length;
            Vector segmentVector = lineSegment.SecondPoint - lineSegment.FirstPoint;
            double interpolatedX = lineSegment.FirstPoint.X + interpolationFactor * segmentVector[0];
            double interpolatedY = lineSegment.FirstPoint.Y + interpolationFactor * segmentVector[1];

            return new Point2D(interpolatedX, interpolatedY);
        }

        /// <summary>
        /// Determines the intersection point of a line through the points <paramref name="point1"/> and
        /// <paramref name="point2"/> with a vertical line at x=<paramref name="x"/>. If 
        /// <paramref name="point1"/> equals <paramref name="point2"/>, then no intersection point 
        /// will be returned.
        /// </summary>
        /// <param name="point1">A <see cref="Point2D"/> which the line passes through.</param>
        /// <param name="point2">Another <see cref="Point2D"/> which the line passes through.</param>
        /// <param name="x">The x coordinate of the vertical line.</param>
        /// <returns>The intersection point between the line through <paramref name="point1"/> and
        /// <paramref name="point2"/> and the vertical line at x=<paramref name="x"/>; or null if
        /// the line through <paramref name="point1"/> and <paramref name="point2"/> is vertical or
        /// the points are equal.</returns>
        private static Point2D LineIntersectionWithVerticalLine(Point2D point1, Point2D point2, double x)
        {
            var verticalLineFirstPoint = new Point2D(x, 0);
            var verticalLineSecondPoint = new Point2D(x, 1);

            try
            {
                return LineIntersectionWithLine(point1, point2, verticalLineFirstPoint, verticalLineSecondPoint);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}