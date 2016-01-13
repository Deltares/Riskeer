using System;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Data
{
    /// <summary>
    /// This class represents data which is represented as a line on <see cref="BaseChart"/>.
    /// </summary>
    public class LineData : ISeries
    {
        private LineSeries series = new LineSeries();

        /// <summary>
        /// Creates a new instance of <see cref="LineData"/>.
        /// </summary>
        /// <param name="points">A <see cref="Collection{T}"/> of <see cref="Tuple{T,T}"/> which represents points 
        /// which when connected form a line.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public LineData(Collection<Tuple<double, double>> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points", "A point collection is required when creating ChartData.");
            }
            series.ItemsSource = points;
            series.Mapping = TupleToDataPoint;
        }

        private DataPoint TupleToDataPoint(object obj)
        {
            var point = (Tuple<double, double>) obj;
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