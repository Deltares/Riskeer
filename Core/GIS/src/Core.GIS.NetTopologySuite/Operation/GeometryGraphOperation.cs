using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Algorithm;
using Core.GIS.NetTopologySuite.GeometriesGraph;

namespace Core.GIS.NetTopologySuite.Operation
{
    /// <summary>
    /// The base class for operations that require <c>GeometryGraph</c>s.
    /// </summary>
    public class GeometryGraphOperation
    {
        /// <summary>
        /// 
        /// </summary>
        protected IPrecisionModel resultPrecisionModel;

        /// <summary>
        /// The operation args into an array so they can be accessed by index.
        /// </summary>
        protected GeometryGraph[] arg;

        private LineIntersector li = new RobustLineIntersector();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g0"></param>
        /// <param name="g1"></param>
        public GeometryGraphOperation(IGeometry g0, IGeometry g1)
        {
            // use the most precise model for the result
            ComputationPrecision = g0.PrecisionModel.CompareTo(g1.PrecisionModel) >= 0 ? g0.PrecisionModel : g1.PrecisionModel;

            arg = new GeometryGraph[2];
            arg[0] = new GeometryGraph(0, g0);
            arg[1] = new GeometryGraph(1, g1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g0"></param>
        public GeometryGraphOperation(IGeometry g0)
        {
            ComputationPrecision = g0.PrecisionModel;

            arg = new GeometryGraph[1];
            arg[0] = new GeometryGraph(0, g0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public IGeometry GetArgGeometry(int i)
        {
            return arg[i].Geometry;
        }

        /// <summary>
        /// 
        /// </summary>
        protected LineIntersector lineIntersector
        {
            get
            {
                return li;
            }
            set
            {
                li = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected IPrecisionModel ComputationPrecision
        {
            get
            {
                return resultPrecisionModel;
            }
            set
            {
                resultPrecisionModel = value;
                lineIntersector.PrecisionModel = resultPrecisionModel;
            }
        }
    }
}