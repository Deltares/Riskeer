using System;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Data
{
    /// <summary>
    /// This class represents data which is represented as points.
    /// </summary>
    public class PointData : DataSeries
    {
        /// <summary>
        /// Creates a new instance of <see cref="PointData"/>.
        /// </summary>
        /// <param name="points">A <see cref="Collection{T}"/> of <see cref="Tuple{T1,T2}"/> which represents points in space.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public PointData(Collection<Tuple<double, double>> points)
        {
            var series = new LineSeries();
            if (points == null)
            {
                throw new ArgumentNullException("points", "A point collection is required when creating ChartData.");
            }
            series.ItemsSource = points;
            series.Mapping = TupleToDataPoint;
            series.LineStyle = LineStyle.None;
            series.MarkerType = MarkerType.Circle;

            Series = series;
        }
    }
}