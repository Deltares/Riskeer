using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// This class represents data in 2D space which is visible as a line.
    /// </summary>
    public class LineData : ChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="LineData"/>.
        /// </summary>
        /// <param name="points">A <see cref="Collection{T}"/> of <see cref="Tuple{T1,T2}"/> which forms a line
        /// in 2D space.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public LineData(IEnumerable<Tuple<double, double>> points) : base(points)
        {
        }
    }
}