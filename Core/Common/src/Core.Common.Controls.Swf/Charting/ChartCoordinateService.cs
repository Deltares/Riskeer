using System;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Geometries;

namespace Core.Common.Controls.Swf.Charting
{
    public class ChartCoordinateService
    {
        public ChartCoordinateService(IChart chart)
        {
            Chart = chart;
        }

        public static int ToDeviceX(IChart chart, double x)
        {
            return HorizontalReferenceAxis(chart).CalcPosValue(x);
        }

        public static int ToDeviceY(IChart chart, double y)
        {
            return VerticalReferenceAxis(chart).CalcPosValue(y);
        }

        public static int ToDeviceWidth(IChart chart, double x)
        {
            var xAxis = HorizontalReferenceAxis(chart);
            return Math.Abs(xAxis.CalcPosValue(x) - xAxis.CalcPosValue(0));
        }

        public static int ToDeviceHeight(IChart chart, double y)
        {
            var yAxis = VerticalReferenceAxis(chart);
            return Math.Abs(yAxis.CalcPosValue(y) - yAxis.CalcPosValue(0));
        }

        public static double ToWorldX(IChart chart, int x)
        {
            return HorizontalReferenceAxis(chart).CalcPosPoint(x);
        }

        public static double ToWorldY(IChart chart, int y)
        {
            return VerticalReferenceAxis(chart).CalcPosPoint(y);
        }

        public static double ToWorldWidth(IChart chart, int x)
        {
            var xAxis = HorizontalReferenceAxis(chart);
            return Math.Abs(xAxis.CalcPosPoint(x) - xAxis.CalcPosPoint(0));
        }

        public static double ToWorldHeight(IChart chart, int y)
        {
            var yAxis = VerticalReferenceAxis(chart);
            return Math.Abs(yAxis.CalcPosPoint(y) - yAxis.CalcPosPoint(0));
        }

        public static ICoordinate ToCoordinate(IChart chart, int x, int y)
        {
            return new Coordinate(ToWorldX(chart, x), ToWorldY(chart, y));
        }

        public int ToDeviceX(double x)
        {
            return ToDeviceX(Chart, x);
        }

        public int ToDeviceY(double y)
        {
            return ToDeviceY(Chart, y);
        }

        public int ToDeviceWidth(double x)
        {
            return ToDeviceWidth(Chart, x);
        }

        public int ToDeviceHeight(double y)
        {
            return ToDeviceHeight(Chart, y);
        }

        public double ToWorldX(int x)
        {
            return ToWorldX(Chart, x);
        }

        public double ToWorldY(int y)
        {
            return ToWorldY(Chart, y);
        }

        public double ToWorldWidth(int x)
        {
            return ToWorldWidth(Chart, x);
        }

        public double ToWorldHeight(int y)
        {
            return ToWorldHeight(Chart, y);
        }

        public ICoordinate ToCoordinate(int x, int y)
        {
            return ToCoordinate(Chart, x, y);
        }

        private IChart Chart { get; set; }

        private static IChartAxis VerticalReferenceAxis(IChart chart)
        {
            return chart.LeftAxis;
        }

        private static IChartAxis HorizontalReferenceAxis(IChart chart)
        {
            return chart.BottomAxis;
        }
    }
}