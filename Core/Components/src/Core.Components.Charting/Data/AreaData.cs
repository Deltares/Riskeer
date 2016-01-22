using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// This class represents data in 2D space which forms a closed area.
    /// </summary>
    public class AreaData : PointBasedChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="AreaData"/>.
        /// </summary>
        /// <param name="points">A <see cref="Collection{T}"/> of <see cref="Tuple{T1,T2}"/> as (X,Y) points.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public AreaData(IEnumerable<Tuple<double, double>> points) : base(points) {}
    }
}