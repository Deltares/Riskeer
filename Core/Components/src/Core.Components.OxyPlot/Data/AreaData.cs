using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Data
{
    /// <summary>
    /// This class represents data which is represented as an area on.
    /// </summary>
    public class AreaData : DataSeries
    {
        /// <summary>
        /// Creates a new instance of <see cref="AreaData"/>.
        /// </summary>
        /// <param name="points">A <see cref="Collection{T}"/> of <see cref="Tuple{T1,T2}"/> which represents points
        /// which when connected form an area.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public AreaData(IEnumerable<Tuple<double, double>> points)
        {
            var series = new AreaSeries();
            if (points == null)
            {
                throw new ArgumentNullException("points", "A point collection is required when creating ChartData.");
            }
            foreach (var p in points)
            {
                series.Points.Add(TupleToDataPoint(p));
            }
            if (series.Points.Count > 0)
            {
                series.Points2.Add(series.Points[0]);
            }

            Series = series;
        }
    }
}