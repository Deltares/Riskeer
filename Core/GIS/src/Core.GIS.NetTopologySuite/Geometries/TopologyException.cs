using System;
using Core.GIS.GeoApi.Geometries;

namespace Core.GIS.NetTopologySuite.Geometries
{
    /// <summary> 
    /// Indicates an invalid or inconsistent topological situation encountered during processing
    /// </summary>
    public class TopologyException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public TopologyException(string msg) : base(msg)
        {
            Coordinate = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="pt"></param>
        public TopologyException(string msg, ICoordinate pt)
            : base(MsgWithCoord(msg, pt))
        {
            Coordinate = new Coordinate(pt);
        }

        /// <summary>
        /// 
        /// </summary>
        public ICoordinate Coordinate { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        private static string MsgWithCoord(string msg, ICoordinate pt)
        {
            if (pt != null)
            {
                return msg + " [ " + pt + " ]";
            }
            return msg;
        }
    }
}