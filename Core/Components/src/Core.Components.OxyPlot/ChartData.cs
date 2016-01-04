using System;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot
{
    public class ChartData
    {
        private LineSeries series;

        /// <summary>
        /// Creates a new instance of <see cref="ChartData"/>.
        /// </summary>
        /// <param name="points">A <see cref="Collection{T}"/> of <see cref="Tuple{T,T}"/> which represent points on a line.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public ChartData(Collection<Tuple<double,double>> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points", "A point collection is required when creating ChartData.");
            }
            series = new LineSeries
            {
                ItemsSource = points,
                Mapping = point => new DataPoint(((Tuple<double, double>)point).Item1, ((Tuple<double, double>)point).Item2)
            };
        }

        /// <summary>
        /// Adds the information in the <see cref="ChartData"/> as a series of the <paramref name="model"/>.
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