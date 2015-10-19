using System;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace GisSharpBlog.NetTopologySuite.Triangulate.QuadEdge
{
    public class LocateFailureException : Exception
    {
        public LocateFailureException(LineSegment seg)
            : base("Locate failed to converge (at edge: "
                   + seg
                   + ").  Possible causes include invalid Subdivision topology or very close sites")
        {

        }
    }
}