using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data.Calculation
{
    /// <summary>
    /// This class contains general mathematical routines for 2D lines.
    /// </summary>
    public static class Math2D
    {
        /// <summary>
        /// Constant which is used to precision errors in <see cref="double"/> comparisons.
        /// </summary>
        private const double epsilonForComparisons = 1e-8;

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

            return new Point2D
            {
                X = (bOtherLine*cLine - bLine*cOtherLine)/determinant,
                Y = (aLine*cOtherLine - aOtherLine*cLine)/determinant
            };
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
            var verticalLineFirstPoint = new Point2D
            {
                X = x,
                Y = 0
            };
            var verticalLineSecondPoint = new Point2D
            {
                X = x,
                Y = 1
            };

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