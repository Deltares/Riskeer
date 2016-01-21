using System;
using System.Collections.Generic;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// This class represents collections of <see cref="ChartData"/>.
    /// </summary>
    public class ChartDataCollection : ChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartDataCollection"/>.
        /// </summary>
        /// <param name="list">A <see cref="IList{T}"/> of <see cref="ChartData"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is <c>null</c>.</exception>
        public ChartDataCollection(IList<ChartData> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list", "A list collection is required when creating ChartDataCollection.");
            }
            List = list;
        }

        /// <summary>
        /// Gets the list of <see cref="ChartData"/> of the <see cref="ChartDataCollection"/>.
        /// </summary>
        public IList<ChartData> List { get; private set; }
    }
}