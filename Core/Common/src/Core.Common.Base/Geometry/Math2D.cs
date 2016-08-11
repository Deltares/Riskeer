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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Properties;
using MathNet.Numerics.LinearAlgebra;
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
            var cLine = aLine*line1Point1.X + bLine*line1Point1.Y;

            var aOtherLine = line2Point2.Y - line2Point1.Y;
            var bOtherLine = line2Point1.X - line2Point2.X;
            var cOtherLine = aOtherLine*line2Point1.X + bOtherLine*line2Point1.Y;

            var determinant = aLine*bOtherLine - aOtherLine*bLine;
            if (Math.Abs(determinant) < epsilonForComparisons)
            {
                return null;
            }

            var x = (bOtherLine*cLine - bLine*cOtherLine)/determinant;
            var y = (aLine*cOtherLine - aOtherLine*cLine)/determinant;
            return new Point2D(x, y);
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
        /// <param name="verticalLineX">The X-coordinate of the vertical line.</param>
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
        /// Calculates the length of a line defined as a collection of <see cref="Point2D"/>.
        /// </summary>
        /// <param name="points">The points that make up a 2D line.</param>
        /// <returns>The sum of the distances between consecutive points.</returns>
        public static double Length(IEnumerable<Point2D> points)
        {
            double length = 0;
            Point2D previousPoint = null;

            foreach (Point2D point in points)
            {
                if (previousPoint != null)
                {
                    length += previousPoint.GetEuclideanDistanceTo(point);
                }
                previousPoint = point;
            }

            return length;
        }

        /// <summary>
        /// Calculates the intersection between two 2D segments.
        /// </summary>
        /// <param name="segment1">The first 2D segment.</param>
        /// <param name="segment2">The second 2D segment.</param>
        /// <returns>The intersection calculation result.</returns>
        /// <remarks>Implementation from http://geomalgorithms.com/a05-_intersect-1.html
        /// based on method <c>intersect2D_2Segments</c>.</remarks>
        public static Segment2DIntersectSegment2DResult GetIntersectionBetweenSegments(Segment2D segment1, Segment2D segment2)
        {
            Vector<double> u = segment1.SecondPoint - segment1.FirstPoint;
            Vector<double> v = segment2.SecondPoint - segment2.FirstPoint;
            Vector<double> w = segment1.FirstPoint - segment2.FirstPoint;
            double d = PerpDotProduct(u, v);

            if (Math.Abs(d) < epsilonForComparisons)
            {
                // Segments can be considered parallel...
                if (AreCollinear(u, v, w))
                {
                    // ... and collinear ...
                    if (IsSegmentAsPointIntersectionDegenerateScenario(segment1, segment2))
                    {
                        // ... but either or both segments are point degenerates:
                        return HandleSegmentAsPointIntersectionDegenerates(segment1, segment2);
                    }

                    // ... so there is a possibility of overlapping or connected lines:
                    return HandleCollinearSegmentIntersection(segment1, segment2, v, w);
                }

                // ... but not collinear, so no intersection possible:
                return Segment2DIntersectSegment2DResult.CreateNoIntersectResult();
            }
            else
            {
                // Segments are at an angle and may intersect:
                double sI = PerpDotProduct(v, w)/d;
                if (sI < 0.0 || sI > 1.0)
                {
                    return Segment2DIntersectSegment2DResult.CreateNoIntersectResult();
                }

                double tI = PerpDotProduct(u, w)/d;
                if (tI < 0.0 || tI > 1.0)
                {
                    return Segment2DIntersectSegment2DResult.CreateNoIntersectResult();
                }

                Point2D intersectionPoint = segment1.FirstPoint + u.Multiply(sI);
                return Segment2DIntersectSegment2DResult.CreateIntersectionResult(intersectionPoint);
            }
        }

        /// <summary>
        /// Gets the interpolated point between the two end points of the <paramref name="lineSegment"/> at
        /// the <paramref name="fraction"/> of the length from the first end point.
        /// </summary>
        /// <param name="lineSegment">The segment to interpolate over.</param>
        /// <param name="fraction">The fraction of the length of the segment where to obtain a new point.</param>
        /// <returns>A new <see cref="Point2D"/> at the interpolated point.</returns>
        public static Point2D GetInterpolatedPointAtFraction(Segment2D lineSegment, double fraction)
        {
            if (double.IsNaN(fraction)|| fraction < 0.0 || fraction > 1.0)
            {
                throw new ArgumentOutOfRangeException("fraction", "Fraction needs to be defined in range [0.0, 1.0] in order to reliably interpolate.");
            }
            Vector<double> segmentVector = lineSegment.SecondPoint - lineSegment.FirstPoint;
            return lineSegment.FirstPoint + segmentVector.Multiply(fraction);
        }

        /// <summary>
        /// Determines if two parallel vectors are collinear.
        /// </summary>
        /// <param name="vector1">The first 2D vector.</param>
        /// <param name="vector2">The second 2D vector.</param>
        /// <param name="tailsVector">The vector from the tail of <paramref name="vector2"/>
        /// to the tail of <paramref name="vector1"/>.</param>
        /// <returns><c>True</c> if the vectors are collinear, <c>false</c> otherwise.</returns>
        private static bool AreCollinear(Vector<double> vector1, Vector<double> vector2, Vector<double> tailsVector)
        {
            return Math.Abs(PerpDotProduct(vector1, tailsVector)) < epsilonForComparisons &&
                   Math.Abs(PerpDotProduct(vector2, tailsVector)) < epsilonForComparisons;
        }

        private static Segment2DIntersectSegment2DResult HandleCollinearSegmentIntersection(Segment2D segment1, Segment2D segment2, Vector<double> v, Vector<double> w)
        {
            double t0, t1;
            Vector<double> w2 = segment1.SecondPoint - segment2.FirstPoint;
            if (v[0] != 0.0)
            {
                t0 = w[0]/v[0];
                t1 = w2[0]/v[0];
            }
            else
            {
                t0 = w[1]/v[1];
                t1 = w2[1]/v[1];
            }
            // Require t0 to be smaller than t1, swapping if needed:
            if (t0 > t1)
            {
                double tempSwapVariable = t0;
                t0 = t1;
                t1 = tempSwapVariable;
            }
            if (t0 > 1.0 || t1 < 0.0)
            {
                // There is no overlap:
                return Segment2DIntersectSegment2DResult.CreateNoIntersectResult();
            }

            t0 = Math.Max(0.0, t0);
            t1 = Math.Min(1.0, t1);
            Point2D intersectionPoint1 = segment2.FirstPoint + v.Multiply(t0);
            if (Math.Abs(t0 - t1) < epsilonForComparisons)
            {
                // Segments intersect at a point:
                return Segment2DIntersectSegment2DResult.CreateIntersectionResult(intersectionPoint1);
            }
            else
            {
                // Segments overlap:
                Point2D intersectionPoint2 = segment2.FirstPoint + v.Multiply(t1);
                return Segment2DIntersectSegment2DResult.CreateOverlapResult(intersectionPoint1, intersectionPoint2);
            }
        }

        private static bool IsSegmentAsPointIntersectionDegenerateScenario(Segment2D segment1, Segment2D segment2)
        {
            return IsSegmentActuallyPointDegenerate(segment1) || IsSegmentActuallyPointDegenerate(segment2);
        }

        private static bool IsSegmentActuallyPointDegenerate(Segment2D segment)
        {
            return segment.Length < epsilonForComparisons;
        }

        private static Segment2DIntersectSegment2DResult HandleSegmentAsPointIntersectionDegenerates(Segment2D segment1, Segment2D segment2)
        {
            bool segment1IsPointDegenerate = IsSegmentActuallyPointDegenerate(segment1);
            bool segment2IsPointDegenerate = IsSegmentActuallyPointDegenerate(segment2);

            if (segment1IsPointDegenerate)
            {
                if (segment2IsPointDegenerate)
                {
                    // Both segments can be considered Point2D
                    return segment1.FirstPoint.Equals(segment2.FirstPoint) ?
                               Segment2DIntersectSegment2DResult.CreateIntersectionResult(segment1.FirstPoint) :
                               Segment2DIntersectSegment2DResult.CreateNoIntersectResult();
                }
                {
                    return IsPointInCollinearSegment(segment1.FirstPoint, segment2) ?
                               Segment2DIntersectSegment2DResult.CreateIntersectionResult(segment1.FirstPoint) :
                               Segment2DIntersectSegment2DResult.CreateNoIntersectResult();
                }
            }

            return IsPointInCollinearSegment(segment2.FirstPoint, segment1) ?
                       Segment2DIntersectSegment2DResult.CreateIntersectionResult(segment2.FirstPoint) :
                       Segment2DIntersectSegment2DResult.CreateNoIntersectResult();
        }

        private static bool IsPointInCollinearSegment(Point2D point, Segment2D colinearSegment)
        {
            if (colinearSegment.IsVertical())
            {
                double minY = Math.Min(colinearSegment.FirstPoint.Y, colinearSegment.SecondPoint.Y);
                double maxY = Math.Max(colinearSegment.FirstPoint.Y, colinearSegment.SecondPoint.Y);
                return minY <= point.Y && point.Y <= maxY;
            }
            else
            {
                double minX = Math.Min(colinearSegment.FirstPoint.X, colinearSegment.SecondPoint.X);
                double maxX = Math.Max(colinearSegment.FirstPoint.X, colinearSegment.SecondPoint.X);
                return minX <= point.X && point.X <= maxX;
            }
        }

        /// <summary>
        /// Performs the dot product between a vector and a perpendicularized vector.
        /// </summary>
        /// <param name="vector1">The vector.</param>
        /// <param name="vector2">The vector that will be made perpendicular before doing
        /// the dot product operation.</param>
        /// <returns>The dot product between a vector and a perpendicularized vector.</returns>
        private static double PerpDotProduct(Vector<double> vector1, Vector<double> vector2)
        {
            Vector perpendicularVectorForVector2 = ToPerpendicular(vector2);
            return vector1.DotProduct(perpendicularVectorForVector2);
        }

        /// <summary>
        /// Creates a new <see cref="Vector"/> based on a vector that is perpendicular.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>A vector of the same length as <paramref name="vector"/>.</returns>
        private static Vector ToPerpendicular(Vector<double> vector)
        {
            return new DenseVector(new[]
            {
                -vector[1],
                vector[0]
            });
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
            var interpolationFactor = splitDistance/lineSegment.Length;
            return GetInterpolatedPointAtFraction(lineSegment, interpolationFactor);
        }

        /// <summary>
        /// Determines the intersection point of a line through the points <paramref name="point1"/> and
        /// <paramref name="point2"/> with a vertical line at x=<paramref name="x"/>. If 
        /// <paramref name="point1"/> equals <paramref name="point2"/>, then no intersection point 
        /// will be returned.
        /// </summary>
        /// <param name="point1">A <see cref="Point2D"/> which the line passes through.</param>
        /// <param name="point2">Another <see cref="Point2D"/> which the line passes through.</param>
        /// <param name="x">The X-coordinate of the vertical line.</param>
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