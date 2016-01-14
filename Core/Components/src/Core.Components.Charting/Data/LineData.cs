using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// This class represents data which is represented as a line.
    /// </summary>
    public class LineData : IChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="LineData"/>.
        /// </summary>
        /// <param name="points">A <see cref="Collection{T}"/> of <see cref="Tuple{T1,T2}"/> which represents points 
        /// which, when connected, form a line.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public LineData(Collection<Tuple<double, double>> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points", "A point collection is required when creating ChartData.");
            }
            Points = points.ToArray();
            IsVisible = true;
        }

        public bool IsVisible { get; set; }

        public IEnumerable<Tuple<double, double>> Points { get; private set; }
    }
}