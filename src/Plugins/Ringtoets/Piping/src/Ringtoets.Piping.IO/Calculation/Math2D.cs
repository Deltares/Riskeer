using System;

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
        /// Tries to find the point where two line segments intersect with each other. Note that if the lines are parallel, no intersection point is returned.
        /// </summary>
        /// <param name="segmentsX">X coordinates of the segments. Should have matching y coordinates in <paramref name="segmentsY"/>.</param>
        /// <param name="segmentsY">Y coordinates of the segments. Should have matching x coordinates in <paramref name="segmentsX"/>.</param>
        /// <returns></returns>
        public static double[] LineSegmentIntersectionWithLineSegment(double[] segmentsX, double[] segmentsY)
        {
            var numberOfPoints = 4;
            if (segmentsX.Length != numberOfPoints || segmentsY.Length != numberOfPoints)
            {
                throw new ArgumentException(String.Format("Collections of segments' x and y coordinates need to have length of {0}.", numberOfPoints));
            }
            var extraPolatedIntersectionPoint = LineSegmentIntersectionWithLine(segmentsX, segmentsY);

            if (extraPolatedIntersectionPoint.Length == 2)
            {
                var onSecondSegment = IsBetween(new[]
                {
                    segmentsX[2],
                    segmentsX[3],
                    extraPolatedIntersectionPoint[0]
                }, new[]
                {
                    segmentsY[2],
                    segmentsY[3],
                    extraPolatedIntersectionPoint[1]
                });
                if (onSecondSegment)
                {
                    return extraPolatedIntersectionPoint;
                }
            }
            return new double[0];
        }

        /// <summary>
        /// Tries to find the point where the line segment intersects with the line. Note that if the segment and the line are parallel, no intersection point is returned.
        /// </summary>
        /// <param name="segmentsX">X coordinates of the segment and line. Should have matching y coordinates in <paramref name="segmentsY"/>.</param>
        /// <param name="segmentsY">Y coordinates of the segment and line. Should have matching x coordinates in <paramref name="segmentsX"/>.</param>
        /// <returns></returns>
        public static double[] LineSegmentIntersectionWithLine(double[] segmentsX, double[] segmentsY)
        {
            var extraPolatedIntersectionPoint = LineIntersectionWithLine(segmentsX, segmentsY);

            if (extraPolatedIntersectionPoint.Length == 2)
            {
                var onFirstSegment = IsBetween(new[]
                {
                    segmentsX[0],
                    segmentsX[1],
                    extraPolatedIntersectionPoint[0]
                }, new[]
                {
                    segmentsY[0],
                    segmentsY[1],
                    extraPolatedIntersectionPoint[1]
                });
                if (onFirstSegment)
                {
                    return extraPolatedIntersectionPoint;
                }
            }
            return new double[0];
        }

        /// <remarks>Taken from: https://www.topcoder.com/community/data-science/data-science-tutorials/geometry-concepts-line-intersection-and-its-applications/ .</remarks>
        private static double[] LineIntersectionWithLine(double[] linesX, double[] linesY)
        {
            var numberOfPoints = 4;
            if (linesX.Length != numberOfPoints || linesY.Length != numberOfPoints)
            {
                throw new ArgumentException(String.Format("Collections of lines' x and y coordinates need to have length of {0}", numberOfPoints));
            }

            var aLine = linesY[1] - linesY[0];
            var bLine = linesX[0] - linesX[1];
            var cLine = aLine * linesX[0] + bLine * linesY[0];

            var aOtherLine = linesY[3] - linesY[2];
            var bOtherLine = linesX[2] - linesX[3];
            var cOtherLine = aOtherLine * linesX[2] + bOtherLine * linesY[2];

            var determinant = aLine*bOtherLine - aOtherLine*bLine;
            if (Math.Abs(determinant) < EpsilonForComparisons)
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
            if (Math.Abs(crossProduct) > EpsilonForComparisons)
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