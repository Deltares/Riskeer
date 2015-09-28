using GeoAPI.Geometries;

namespace GisSharpBlog.NetTopologySuite.Triangulate
{
    /// <summary>
    /// An interface for factories which create a {@link ConstraintVertex}
    /// </summary>
    /// <author>Martin Davis</author>
    public interface ConstraintVertexFactory
    {
        ConstraintVertex CreateVertex(ICoordinate p, Segment constraintSeg);
    }
}