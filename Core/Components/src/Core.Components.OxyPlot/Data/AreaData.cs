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
    public class AreaData : IChartData
    {
        private AreaSeries series;
        
        /// <summary>
        /// Creates a new instance of <see cref="AreaData"/>.
        /// </summary>
        /// <param name="points">A <see cref="Collection{T}"/> of <see cref="Tuple{T,T}"/> which represent points on a line.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public AreaData(Collection<Tuple<double, double>> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points", "A point collection is required when creating ChartData.");
            }
            series = new AreaSeries
            {
                MarkerType = MarkerType.Cross,
                MarkerStroke = OxyColors.Black
            };
            foreach (var p in points)
            {
                series.Points.Add(Mapping(p));
            }
            series.Points2.Add(Mapping(points.First()));
        }

        private DataPoint Mapping(Tuple<double, double> point)
        {
            return new DataPoint(point.Item1, point.Item2);
        }

        /// <summary>
        /// Adds the information in the <see cref="AreaData"/> as a series of the <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="PlotModel"/> to add a series to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        public void AddTo(PlotModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "A model is required to add points to.");
            }
            model.Series.Add(series);
        }
    }
}