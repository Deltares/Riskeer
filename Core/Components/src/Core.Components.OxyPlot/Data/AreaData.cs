using System;
using System.Collections.ObjectModel;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Data
{
    /// <summary>
    /// This class represents data which is represented as an area on <see cref="BaseChart"/>.
    /// </summary>
    public class AreaData : ISeries
    {
        private readonly AreaSeries series = new AreaSeries();

        /// <summary>
        /// Creates a new instance of <see cref="AreaData"/>.
        /// </summary>
        /// <param name="points">A <see cref="Collection{T}"/> of <see cref="Tuple{T,T}"/> which represents points
        /// which when connected form an area.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public AreaData(Collection<Tuple<double, double>> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points", "A point collection is required when creating ChartData.");
            }
            foreach (var p in points)
            {
                series.Points.Add(TupleToDataPoint(p));
            }
            if (points.Count > 0)
            {
                series.Points2.Add(TupleToDataPoint(points.First()));
            }
        }

        private DataPoint TupleToDataPoint(Tuple<double, double> point)
        {
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