using System;

namespace DelftTools.Controls.Swf.Charting.Series
{
    public static class ChartSeriesFactory
    {
        public static ILineChartSeries CreateLineSeries(IChartSeries baseSeries = null)
        {
            return baseSeries != null
                       ? new LineChartSeries(baseSeries)
                       : new LineChartSeries();
        }

        public static IPointChartSeries CreatePointSeries(IChartSeries baseSeries = null)
        {
            return baseSeries != null
                       ? new PointChartSeries(baseSeries)
                       : new PointChartSeries();
        }

        public static IAreaChartSeries CreateAreaSeries(IChartSeries baseSeries = null)
        {
            return baseSeries != null
                       ? new AreaChartSeries(baseSeries)
                       : new AreaChartSeries();
        }

        public static IChartSeries CreateBarSeries(IChartSeries baseSeries = null)
        {
            return baseSeries != null
                       ? new BarSeries(baseSeries)
                       : new BarSeries();
        }

        public static IPolygonChartSeries CreatePolygonSeries(IChartSeries baseSeries = null)
        {
            return baseSeries != null
                       ? new PolygonChartSeries(baseSeries)
                       : new PolygonChartSeries();
        }

        public static IChartSeries CreateSeries(ChartSeriesType chartSeriesType)
        {
            switch (chartSeriesType)
            {
                case ChartSeriesType.PointSeries:
                    return CreatePointSeries();
                case ChartSeriesType.LineSeries:
                    return CreateLineSeries();
                case ChartSeriesType.AreaSeries:
                    return CreateAreaSeries();
                case ChartSeriesType.BarSeries:
                    return CreateBarSeries();
                case ChartSeriesType.PolygonSeries:
                    return CreatePolygonSeries();
                default:
                    throw new ArgumentOutOfRangeException("unsupported chartSeriesType");
            }
        }
    }
}