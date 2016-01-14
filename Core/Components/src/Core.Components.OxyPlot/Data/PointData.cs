using System;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Data
{
    /// <summary>
    /// This class represents data which is represented as points on <see cref="BaseChart"/>.
    /// </summary>
    public class PointData : ISeries
    {
        private readonly LineSeries series = new LineSeries();

        /// <summary>
        /// Creates a new instance of <see cref="PointData"/>.
        /// </summary>
        /// <param name="points">A <see cref="Collection{T}"/> of <see cref="Tuple{T,T}"/> which represents points in space.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public PointData(Collection<Tuple<double, double>> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points", "A point collection is required when creating ChartData.");
            }
            series.ItemsSource = points;
            series.Mapping = TupleToDataPoint;
            series.LineStyle = LineStyle.None;
            series.MarkerType = MarkerType.Circle;
        }

        private DataPoint TupleToDataPoint(object obj)
        {
            var point = (Tuple<double, double>)obj;
            return new DataPoint(point.Item1, point.Item2);
        }

        public Series Series
        {
            get
            {
                return series;
            }
        }

        public bool IsVisible
        {
            get
            {
                return series.IsVisible;
            }
            set
            {
                series.IsVisible = value;
            }
        }
    }
}