using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace GisSharpBlog.NetTopologySuite.Simplify
{
    /// <summary>
    /// A LineSegment which is tagged with its location in a <c>Geometry</c>.
    /// Used to index the segments in a point and recover the segment locations
    /// from the index.
    /// </summary>
    public class TaggedLineSegment : LineSegment
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        public TaggedLineSegment(ICoordinate p0, ICoordinate p1, IGeometry parent, int index)
            : base(p0, p1)
        {
            Parent = parent;
            Index = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        public TaggedLineSegment(ICoordinate p0, ICoordinate p1)
            : this(p0, p1, null, -1) {}

        /// <summary>
        /// 
        /// </summary>
        public IGeometry Parent { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; private set; }
    }
}