using System;
using System.Collections.Generic;
using Core.Components.Charting.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// The interface for a converter which converts <see cref="ChartData"/> into <see cref="Series"/>.
    /// </summary>
    public interface IChartDataConverter {

        /// <summary>
        /// Checks whether the <see cref="IChartDataConverter"/> can convert the <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="ChartData"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref name="data"/> can be converted by the
        /// <see cref="IChartDataConverter"/>, <c>false</c> otherwise.</returns>
        bool CanConvertSeries(ChartData data);

        /// <summary>
        /// Creates one or more <see cref="Series"/> based on the <paramref name="data"/> that was given.
        /// </summary>
        /// <param name="data">The data to transform into a <see cref="Series"/>.</param>
        /// <returns>A new <see cref="IList{T}"/> of <see cref="Series"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <see cref="CanConvertSeries"/>
        /// returns <c>false</c>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/>
        /// is <c>null</c>.</exception>
        IList<Series> Convert(ChartData data);
    }
}