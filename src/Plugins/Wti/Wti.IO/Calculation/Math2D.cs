using System;

namespace Wti.IO.Calculation
{
    public class Math2D
    {
        private const double epsilonForComparisons = 1e-8;

        /// <summary>
        /// Determines whether two line segments intersect with each other. If the lines are parallel, no intersection point is returned.
        /// </summary>
        /// <param name="segmentX">X coordinates of the first segment. Should have matching y coordinates in <paramref name="segmentY"/>.</param>
        /// <param name="segmentY">Y coordinates of the first segment. Should have matching x coordinates in <paramref name="segmentX"/>.</param>
        /// <param name="otherX">X coordinates of the second segment. Should have matching y coordinates in <paramref name="otherY"/>.</param>
        /// <param name="otherY">Y coordinates of the second segment. Should have matching x coordinates in <paramref name="otherX"/>.</param>
        /// <returns></returns>
        public static double[] LineSegmentIntersectionWithLineSegment(double[] segmentX, double[] segmentY, double[] otherX, double[] otherY)
        {
            var numberOfPoints = 2;
            if (segmentX.Length != numberOfPoints || segmentY.Length != numberOfPoints || otherX.Length != numberOfPoints || otherY.Length != numberOfPoints)
            {
                throw new ArgumentException("Collections of segments' x and y coordinates need to have length of 2");
            }
            var extraPolatedIntersectionPoint = LineIntersectionWithLine(segmentX, segmentY, otherX, otherY);

            if (extraPolatedIntersectionPoint.Length == 2)
            {
                var onFirstSegment = IsBetween(new[]
                {
                    segmentX[0],
                    segmentX[1],
                    extraPolatedIntersectionPoint[0]
                }, new[]
                {
                    segmentY[0],
                    segmentY[1],
                    extraPolatedIntersectionPoint[1]
                });
                var onSecondSegment = IsBetween(new[]
                {
                    otherX[0],
                    otherX[1],
                    extraPolatedIntersectionPoint[0]
                }, new[]
                {
                    otherY[0],
                    otherY[1],
                    extraPolatedIntersectionPoint[1]
                });
                if (onFirstSegment && onSecondSegment)
                {
                    return extraPolatedIntersectionPoint;
                }
            }
            return new double[0];
        }

        /// <remarks>Taken from: https://www.topcoder.com/community/data-science/data-science-tutorials/geometry-concepts-line-intersection-and-its-applications/ .</remarks>
        private static double[] LineIntersectionWithLine(double[] lineX, double[] lineY, double[] otherLineX, double[] otherLineY)
        {
            var numberOfPoints = 2;
            if (lineX.Length != numberOfPoints || lineY.Length != numberOfPoints || otherLineX.Length != numberOfPoints || otherLineY.Length != numberOfPoints)
            {
                throw new ArgumentException("Collections of lines' x and y coordinates need to have length of 2");
            }

            var aLine = lineY[1] - lineY[0];
            var bLine = lineX[0] - lineX[1];
            var cLine = aLine*lineX[0] + bLine*lineY[0];

            var aOtherLine = otherLineY[1] - otherLineY[0];
            var bOtherLine = otherLineX[0] - otherLineX[1];
            var cOtherLine = aOtherLine * otherLineX[0] + bOtherLine * otherLineY[0];

            var determinant = aLine*bOtherLine - aOtherLine*bLine;
            if (Math.Abs(determinant) < epsilonForComparisons)
            {
                return new double[0];
            }

            return new[]
            {
                (bOtherLine*cLine - bLine*cOtherLine)/determinant,
                (aLine*cOtherLine - aOtherLine*cLine)/determinant
            };
        }

        /// <summary>
        /// Checks whether point <paramref name="x"/>[2],<paramref name="y"/>[2] lies between <paramref name="x"/>[0],<paramref name="y"/>[0] 
        /// and <paramref name="x"/>[1],<paramref name="y"/>[1].
        /// </summary>
        /// <param name="x">X-coordinates of the 3 points.</param>
        /// <param name="y">Y-coordinates of the 3 points.</param>
        /// <returns>True if point <paramref name="x"/>[2],<paramref name="y"/>[2] lies between <paramref name="x"/>[0],<paramref name="y"/>[0] 
        /// and <paramref name="x"/>[1],<paramref name="y"/>[1]. False otherwise.</returns>
        private static bool IsBetween(double[] x, double[] y)
        {
            var crossProduct = (y[2] - y[0])*(x[1] - x[0]) - (x[2] - x[0])*(y[1] - y[0]);
            if (Math.Abs(crossProduct) > epsilonForComparisons)
            {
                return false;
            }

            var dotProduct = (x[2] - x[0])*(x[1] - x[0]) + (y[2] - y[0])*(y[1] - y[0]);
            if (dotProduct < 0)
            {
                return false;
            }

            var squaredLengthSegment = (x[1] - x[0])*(x[1] - x[0]) + (y[1] - y[0])*(y[1] - y[0]);
            return dotProduct <= squaredLengthSegment;
        }
    }
}