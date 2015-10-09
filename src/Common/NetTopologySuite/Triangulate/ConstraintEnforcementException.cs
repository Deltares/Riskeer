using System;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.IO;

#if SILVERLIGHT
using ApplicationException = System.Exception;
#endif

namespace GisSharpBlog.NetTopologySuite.Triangulate
{
    /// <summary>
    /// Indicates a failure during constraint enforcement.
    /// </summary>
    /// <author>Martin Davis</author>
    /// <version>1.0</version>
    public class ConstraintEnforcementException : ApplicationException
    {
        private readonly ICoordinate _pt;

        /// <summary>
        /// Creates a new instance with a given message.
        /// </summary>
        /// <param name="msg">a string</param>
        public ConstraintEnforcementException(string msg)
            : base(msg) {}

        /// <summary>
        /// Creates a new instance with a given message and approximate location.
        /// </summary>
        /// <param name="msg">a string</param>
        /// <param name="pt">the location of the error</param>
        public ConstraintEnforcementException(String msg, ICoordinate pt)
            : base(MsgWithCoord(msg, pt))
        {
            _pt = new Coordinate(pt);
        }

        /// <summary>
        /// Gets the approximate location of this error.
        /// </summary>
        /// <remarks>a location</remarks>
        public ICoordinate Coordinate
        {
            get
            {
                return _pt;
            }
        }

        //private long serialVersionUID = 386496846550080140L;

        private static String MsgWithCoord(String msg, ICoordinate pt)
        {
            if (pt != null)
            {
                return msg + " [ " + WKTWriter.ToPoint(pt) + " ]";
            }
            return msg;
        }
    }
}