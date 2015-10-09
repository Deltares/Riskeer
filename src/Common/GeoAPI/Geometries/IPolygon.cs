namespace GeoAPI.Geometries
{
    public interface IPolygon : ISurface
    {
        ILineString ExteriorRing { get; }

        ILinearRing Shell { get; }

        int NumInteriorRings { get; }

        ILineString[] InteriorRings { get; }

        ILinearRing[] Holes { get; }

        ILineString GetInteriorRingN(int n);
    }
}