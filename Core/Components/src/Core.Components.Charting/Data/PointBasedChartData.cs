using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// Base class for <see cref="ChartData"/> which is based on a collection of points.
    /// </summary>
    public abstract class PointBasedChartData : ChartData {

        /// <summary>
        /// Creates a new instance of <see cref="PointData"/>.
        /// </summary>
        /// <param name="points">A <see cref="Collection{T}"/> of <see cref="Tuple{T1,T2}"/> as (X,Y) points.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        protected PointBasedChartData(IEnumerable<Tuple<double, double>> points)
        {
            if (points == null)
            {
                var message = String.Format("A point collection is required when creating a subclass of {0}.", typeof(PointBasedChartData));
                throw new ArgumentNullException("points", message);
            }
            Points = points.ToArray();
            IsVisible = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ChartData"/> is visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets the collection of points in 2D space.
        /// </summary>
        public IEnumerable<Tuple<double, double>> Points { get; private set; }
    }
}