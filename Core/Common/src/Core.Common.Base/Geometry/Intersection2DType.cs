namespace Core.Common.Base.Geometry
{
    /// <summary>
    /// Describes the type of intersection that has been calculated in 2D Euclidean space.
    /// </summary>
    public enum Intersection2DType
    {
        /// <summary>
        /// No intersections have been found.
        /// </summary>
        NoIntersections,
        /// <summary>
        /// Intersections have been found.
        /// </summary>
        Intersects,
        /// <summary>
        /// There is some overlap between two elements.
        /// </summary>
        Overlapping
    }
}