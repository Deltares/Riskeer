using GeoAPI.Geometries;

namespace GisSharpBlog.NetTopologySuite.Utilities
{
    /// <summary>
    /// A <c>CoordinateFilter</c> that counts the total number of coordinates
    /// in a <c>Geometry</c>.
    /// </summary>
    public class CoordinateCountFilter : ICoordinateFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public CoordinateCountFilter()
        {
            Count = 0;
        }

        /// <summary>
        /// Returns the result of the filtering.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord"></param>
        public void Filter(ICoordinate coord)
        {
            Count++;
        }
    }
}