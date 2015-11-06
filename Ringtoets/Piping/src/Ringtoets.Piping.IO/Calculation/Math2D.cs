using System;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Properties;

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

        /// <remarks>
        /// <para>Taken from: https://www.topcoder.com/community/data-science/data-science-tutorials/geometry-concepts-line-intersection-and-its-applications/ </para>
        /// <para>Based on https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection </para>
        /// </remarks>
        public static Point2D LineIntersectionWithLine(Point2D firstPoint, Point2D secondPoint, Point2D verticalLineFirstPoint, Point2D verticalLineSecondPoint)
        {
            var aLine = secondPoint.Y - firstPoint.Y;
            var bLine = firstPoint.X - secondPoint.X;
            var cLine = aLine * firstPoint.X + bLine * firstPoint.Y;

            var aOtherLine = verticalLineSecondPoint.Y - verticalLineFirstPoint.Y;
            var bOtherLine = verticalLineFirstPoint.X - verticalLineSecondPoint.X;
            var cOtherLine = aOtherLine * verticalLineFirstPoint.X + bOtherLine * verticalLineFirstPoint.Y;

            var determinant = aLine * bOtherLine - aOtherLine * bLine;
            if (Math.Abs(determinant) < EpsilonForComparisons)
            {
                return null;
            }

            return new Point2D {
                X = (bOtherLine*cLine - bLine*cOtherLine)/determinant,
                Y = (aLine*cOtherLine - aOtherLine*cLine)/determinant
            };
        }
    }
}