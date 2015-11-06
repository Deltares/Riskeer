using System;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.IO.Calculation
{
    /// <summary>
    /// This class contains general mathematical routines for 2D lines.
    /// </summary>
    public static class Math2D
    {
        /// <summary>
        /// Constant which is used to precision errors in <see cref="double"/> comparisons.
        /// </summary>
        public const double EpsilonForComparisons = 1e-8;

        /// <summary>
        /// Determines the intersection point of a line which passes through the <paramref name="firstPoint"/> and 
        /// the <paramref name="secondPoint"/>; and a line which passes through the <paramref name="thirdPoint"/>
        /// and the <paramref name="fourthPoint"/>. When the lines are parallel, then <c>null</c> will be returned,
        /// as no intersection point exists.
        /// </summary>
        /// <param name="firstPoint">A <see cref="Point2D"/> which the first line passes through.</param>
        /// <param name="secondPoint">Another <see cref="Point2D"/> which the first line passes through.</param>
        /// <param name="thirdPoint">A <see cref="Point2D"/> which the second line passes through.</param>
        /// <param name="fourthPoint">Another <see cref="Point2D"/> which the second line passes through.</param>
        /// <returns>An <see cref="Point2D"/> with coordinates at the point where the lines intersect. Or <c>null</c> when no
        /// intersection point exists (lines are parallel).</returns>
        /// <remarks>
        /// <para>Taken from: https://www.topcoder.com/community/data-science/data-science-tutorials/geometry-concepts-line-intersection-and-its-applications/ </para>
        /// <para>Based on https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection </para>
        /// </remarks>
        public static Point2D LineIntersectionWithLine(Point2D firstPoint, Point2D secondPoint, Point2D thirdPoint, Point2D fourthPoint)
        {
            var aLine = secondPoint.Y - firstPoint.Y;
            var bLine = firstPoint.X - secondPoint.X;
            var cLine = aLine*firstPoint.X + bLine*firstPoint.Y;

            var aOtherLine = fourthPoint.Y - thirdPoint.Y;
            var bOtherLine = thirdPoint.X - fourthPoint.X;
            var cOtherLine = aOtherLine*thirdPoint.X + bOtherLine*thirdPoint.Y;

            var determinant = aLine*bOtherLine - aOtherLine*bLine;
            if (Math.Abs(determinant) < EpsilonForComparisons)
            {
                return null;
            }

            return new Point2D
            {
                X = (bOtherLine*cLine - bLine*cOtherLine)/determinant,
                Y = (aLine*cOtherLine - aOtherLine*cLine)/determinant
            };
        }
    }
}