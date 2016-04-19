using System.Collections;
using System.Collections.Generic;

using Core.Common.Base.Geometry;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// This class compares the distance of two <see cref="Point2D"/> instances to determine
    /// if there are equal to each other or not. This class shouldn't be used to sort point
    /// instances.
    /// </summary>
    public class Point2DComparerWithTolerance : IComparer<Point2D>, IComparer
    {
        private readonly double tolerance;

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2DComparerWithTolerance"/> class.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        public Point2DComparerWithTolerance(double tolerance)
        {
            this.tolerance = tolerance;
        }

        public int Compare(object x, object y)
        {
            return Compare(x as Point2D, y as Point2D);
        }

        public int Compare(Point2D p0, Point2D p1)
        {
            double diff = p0.GetEuclideanDistanceTo(p1);
            return diff <= tolerance ? 0 : 1;
        }
    }
}