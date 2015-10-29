using System;
using Core.GIS.NetTopologySuite.Geometries;

namespace Core.GIS.NetTopologySuite.Triangulate.QuadEdge
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