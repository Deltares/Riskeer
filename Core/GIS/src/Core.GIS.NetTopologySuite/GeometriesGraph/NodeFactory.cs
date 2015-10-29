using Core.GIS.GeoApi.Geometries;

namespace Core.GIS.NetTopologySuite.GeometriesGraph
{
    /// <summary>
    /// 
    /// </summary>
    public class NodeFactory
    {
        /// <summary> 
        /// The basic node constructor does not allow for incident edges.
        /// </summary>
        /// <param name="coord"></param>
        public virtual Node CreateNode(ICoordinate coord)
        {
            return new Node(coord, null);
        }
    }
}