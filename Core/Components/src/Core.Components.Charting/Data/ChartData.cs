using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// Abstract class for data with the purpose of becoming visible in charting components.
    /// </summary>
    public abstract class ChartData : Observable
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartData"/>.
        /// </summary>
        /// <param name="points">An <see cref="IEnumerable{T}"/> of <see cref="Tuple{T1,T2}"/> as (x,y) points.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        protected ChartData(IEnumerable<Tuple<double, double>> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points", "A point collection is required when creating AreaData.");
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