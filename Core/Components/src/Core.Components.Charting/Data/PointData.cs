using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// This class represents data in 2D space which is visible as points.
    /// </summary>
    public class PointData : ChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="PointData"/>.
        /// </summary>
        /// <param name="points">A <see cref="Collection{T}"/> of <see cref="Tuple{T1,T2}"/> as (X,Y) points.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public PointData(IEnumerable<Tuple<double, double>> points) : base(points)
        {
        }
    }
}