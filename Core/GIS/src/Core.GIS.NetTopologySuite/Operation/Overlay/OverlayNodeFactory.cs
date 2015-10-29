using Core.GIS.GeoApi.Geometries;
using Core.GIS.NetTopologySuite.GeometriesGraph;

namespace Core.GIS.NetTopologySuite.Operation.Overlay
{
    /// <summary>
    /// Creates nodes for use in the <c>PlanarGraph</c>s constructed during
    /// overlay operations.
    /// </summary>
    public class OverlayNodeFactory : NodeFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public override Node CreateNode(ICoordinate coord)
        {
            return new Node(coord, new DirectedEdgeStar());
        }
    }
}