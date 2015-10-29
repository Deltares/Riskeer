using Core.GIS.GeoApi.Geometries;

namespace Core.GIS.NetTopologySuite.Algorithm
{
    /// <summary> 
    /// An interface for classes which test whether a <c>Coordinate</c> lies inside a ring.
    /// </summary>
    public interface IPointInRing
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        bool IsInside(ICoordinate pt);
    }
}