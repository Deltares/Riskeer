namespace Core.GIS.GeoApi.Geometries
{
    public interface ILineString : ICurve
    {
        IPoint GetPointN(int n);

        ICoordinate GetCoordinateN(int n);

        ILineString Reverse();
    }
}