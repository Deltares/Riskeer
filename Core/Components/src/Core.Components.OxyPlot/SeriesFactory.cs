using System;
using System.Linq;
using Core.Components.Charting.Data;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot
{
    public class SeriesFactory
    {
        public Series Create(IChartData data)
        {
            var lineData = data as LineData;
            if (lineData != null)
            {
                return Create(lineData);
            }
            var pointData = data as PointData;
            if (pointData != null)
            {
                return Create(pointData);
            }
            var areaData = data as AreaData;
            if (areaData != null)
            {
                return Create(areaData);
            }
            throw new NotSupportedException(String.Format("IChartData of type {0} is not supported.", data.GetType().Name));
        }

        private Series Create(AreaData data)
        {
            var series = new AreaSeries();
            foreach (var p in data.Points)
            {
                series.Points.Add(TupleToDataPoint(p));
            }
            if (series.Points.Count > 0)
            {
                series.Points2.Add(series.Points[0]);
            }
            return series;
        }

        private Series Create(LineData data)
        {
            var series = new LineSeries
            {
                ItemsSource = data.Points.ToArray(),
                Mapping = TupleToDataPoint
            };
            return series;
        }

        private Series Create(PointData data)
        {
            var series = new LineSeries
            {
                ItemsSource = data.Points.ToArray(),
                Mapping = TupleToDataPoint,
                LineStyle = LineStyle.None,
                MarkerType = MarkerType.Circle
            };
            return series;
        }

        private static DataPoint TupleToDataPoint(object obj)
        {
            var point = (Tuple<double, double>)obj;
            return new DataPoint(point.Item1, point.Item2);
        }
    }
}