using System;
using System.Linq;
using Core.Components.Charting.Data;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot
{
    /// <summary>
    /// This class creates new <see cref="Series"/> objects from <see cref="ChartData"/>.
    /// </summary>
    public class SeriesFactory
    {
        /// <summary>
        /// Creates a new <see cref="Series"/> from the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="ChartData"/> to base the creation of a <see cref="Series"/> upon.</param>
        /// <returns>A new <see cref="Series"/>.</returns>
        public Series Create(ChartData data)
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

        /// <summary>
        /// Creates a new <see cref="AreaSeries"/> for <see cref="AreaData"/>.
        /// </summary>
        /// <param name="data">The <see cref="AreaData"/> to base the new <see cref="AreaSeries"/> upon.</param>
        /// <returns>A new <see cref="AreaSeries"/>.</returns>
        private static AreaSeries Create(AreaData data)
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

        /// <summary>
        /// Creates a new <see cref="LineSeries"/> for <see cref="LineData"/>.
        /// </summary>
        /// <param name="data">The <see cref="LineData"/> to base the new <see cref="LineSeries"/> upon.</param>
        /// <returns>A new <see cref="LineSeries"/>.</returns>
        private static LineSeries Create(LineData data)
        {
            var series = new LineSeries
            {
                ItemsSource = data.Points.ToArray(),
                Mapping = TupleToDataPoint
            };
            return series;
        }

        /// <summary>
        /// Creates a new <see cref="LineSeries"/> with point styling for <see cref="PointData"/>.
        /// </summary>
        /// <param name="data">The <see cref="PointData"/> to base the new <see cref="LineSeries"/> upon.</param>
        /// <returns>A new <see cref="LineSeries"/>.</returns>
        private static LineSeries Create(PointData data)
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

        /// <summary>
        /// Transforms a given object into a <see cref="DataPoint"/>
        /// </summary>
        /// <param name="obj">The object to convert into a <see cref="DataPoint"/>.</param>
        /// <returns>A new <see cref="DataPoint"/> based on <paramref name="obj"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when <paramref name="obj"/> is not of type <see cref="Tuple{T,T}"/> of 
        /// <see cref="Double"/>.</exception>
        private static DataPoint TupleToDataPoint(object obj)
        {
            var point = (Tuple<double, double>)obj;
            return new DataPoint(point.Item1, point.Item2);
        }
    }
}