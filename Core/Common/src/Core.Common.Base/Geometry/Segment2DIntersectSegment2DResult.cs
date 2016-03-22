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
            IntersectionPoints = points;
        }

        /// <summary>
        /// Gets the type of the intersection found.
        /// </summary>
        public Intersection2DType IntersectionType { get; private set; }

        /// <summary>
        /// Gets the intersection points, if any.
        /// </summary>
        /// <remarks>
        /// <para>If <see cref="IntersectionType"/> has a value of <see cref="Intersection2DType.Intersects"/>,
        /// the array holds the single intersection points found.</para>
        /// <para>If <see cref="IntersectionType"/> has a value of <see cref="Intersection2DType.Overlapping"/>,
        /// the array holds the two points defining the overlapping area for both segments.</para>
        /// </remarks>
        public Point2D[] IntersectionPoints { get; private set; }

        /// <summary>
        /// Creates the calculation result for having found no intersections.
        /// </summary>
        public static Segment2DIntersectSegment2DResult CreateNoIntersectResult()
        {
            return new Segment2DIntersectSegment2DResult(Intersection2DType.NoIntersections, new Point2D[0]);
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
            return new Segment2DIntersectSegment2DResult(Intersection2DType.Overlapping, new[]
            {
                start,
                end
            });
        }
    }
}