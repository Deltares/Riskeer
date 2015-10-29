namespace Core.Gis.GeoApi.Geometries
{
    public interface IPoint : IGeometry
    {
        double X { get; }

        double Y { get; }

        ICoordinateSequence CoordinateSequence { get; }
    }
}