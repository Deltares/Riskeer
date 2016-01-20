using System;
using System.Collections.Generic;
using Core.Components.Charting.Data;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// Provides an abstract base class for transforming <see cref="ChartData"/> in specific
    /// <see cref="Series"/> instances.
    /// </summary>
    public abstract class ChartDataConverter
    {
        /// <summary>
        /// Returns the type that the <see cref="ChartDataConverter"/> can convert
        /// into a new <see cref="Series"/> instance.
        /// </summary>
        protected abstract Type SupportedType { get; }

        /// <summary>
        /// Transforms a given object into a <see cref="DataPoint"/>. Can be used as a 
        /// <see cref="DataPointSeries.Mapping"/>.
        /// </summary>
        /// <param name="obj">The object to convert into a <see cref="DataPoint"/>.</param>
        /// <returns>A new <see cref="DataPoint"/> based on <paramref name="obj"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when <paramref name="obj"/> is not
        /// of type <see cref="Tuple"/> of <see cref="double"/>.</exception>
        protected static DataPoint TupleToDataPoint(object obj)
        {
            var point = (Tuple<double, double>) obj;
            return new DataPoint(point.Item1, point.Item2);
        }

        /// <summary>
        /// Checks whether the <see cref="ChartDataConverter"/> can convert the <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="ChartData"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref name="data"/> can be converted by the
        /// <see cref="ChartDataConverter"/>, <c>false</c> otherwise.</returns>
        internal bool CanConvertSeries(ChartData data)
        {
            return data.GetType() == SupportedType;
        }

        /// <summary>
        /// Creates one or more <see cref="Series"/> based on the <paramref name="data"/> that was given.
        /// </summary>
        /// <param name="data">The data to transform into a <see cref="Series"/>.</param>
        /// <returns>A new <see cref="IList{T}"/> of <see cref="Series"/>.</returns>
        internal abstract IList<Series> Convert(ChartData data);
    }
}