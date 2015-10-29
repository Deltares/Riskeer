using System.Collections;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Geometries;

namespace Core.GIS.NetTopologySuite.Operation.Linemerge
{
    /// <summary>
    /// A sequence of <c>LineMergeDirectedEdge</c>s forming one of the lines that will
    /// be output by the line-merging process.
    /// </summary>
    public class EdgeString
    {
        private readonly IGeometryFactory factory;
        private readonly IList directedEdges = new ArrayList();
        private ICoordinate[] coordinates = null;

        /// <summary>
        /// Constructs an EdgeString with the given factory used to convert this EdgeString
        /// to a LineString.
        /// </summary>
        /// <param name="factory"></param>
        public EdgeString(IGeometryFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Adds a directed edge which is known to form part of this line.
        /// </summary>
        /// <param name="directedEdge"></param>
        public void Add(LineMergeDirectedEdge directedEdge)
        {
            directedEdges.Add(directedEdge);
        }

        /// <summary>
        /// Converts this EdgeString into a LineString.
        /// </summary>
        public ILineString ToLineString()
        {
            return factory.CreateLineString(Coordinates);
        }

        /// <summary>
        /// 
        /// </summary>
        private ICoordinate[] Coordinates
        {
            get
            {
                if (coordinates == null)
                {
                    int forwardDirectedEdges = 0;
                    int reverseDirectedEdges = 0;
                    CoordinateList coordinateList = new CoordinateList();
                    IEnumerator i = directedEdges.GetEnumerator();
                    while (i.MoveNext())
                    {
                        LineMergeDirectedEdge directedEdge = (LineMergeDirectedEdge) i.Current;
                        if (directedEdge.EdgeDirection)
                        {
                            forwardDirectedEdges++;
                        }
                        else
                        {
                            reverseDirectedEdges++;
                        }
                        coordinateList.Add(((LineMergeEdge) directedEdge.Edge).Line.Coordinates, false, directedEdge.EdgeDirection);
                    }
                    coordinates = coordinateList.ToCoordinateArray();
                    if (reverseDirectedEdges > forwardDirectedEdges)
                    {
                        CoordinateArrays.Reverse(coordinates);
                    }
                }
                return coordinates;
            }
        }
    }
}