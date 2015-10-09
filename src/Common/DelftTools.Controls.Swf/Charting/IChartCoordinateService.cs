using GeoAPI.Geometries;

namespace DelftTools.Controls.Swf.Charting
{
    public interface IChartCoordinateService
    {
        ICoordinate ToCoordinate(int x, int y);
        int ToDeviceHeight(double y);
        int ToDeviceWidth(double x);
        int ToDeviceX(double x);
        int ToDeviceY(double y);
        double ToWorldHeight(int y);
        double ToWorldWidth(int x);
        double ToWorldX(int x);
        double ToWorldY(int y);
    }
}