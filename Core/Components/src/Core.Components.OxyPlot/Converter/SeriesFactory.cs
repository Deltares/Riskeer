using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Components.Charting.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// This class creates new <see cref="Series"/> objects from <see cref="ChartData"/>.
    /// </summary>
    public class SeriesFactory
    {
        /// <summary>
        /// Collection of converters that the <see cref="SeriesFactory"/> can use to transform <see cref="ChartData"/>.
        /// </summary>
        private readonly IEnumerable<ChartDataConverter> converters = new Collection<ChartDataConverter>
        {
            new AreaDataConverter(),
            new LineDataConverter(),
            new PointDataConverter()
        };

        /// <summary>
        /// Creates a new <see cref="Series"/> from the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="ChartData"/> to base the creation of a <see cref="Series"/> upon.</param>
        /// <returns>A new <see cref="Series"/>.</returns>
        public Series Create(ChartData data)
        {
            foreach (var converter in converters)
            {
                if (converter.CanConvertSeries(data))
                {
                    return converter.Convert(data);
                }
            }
            throw new NotSupportedException(string.Format("IChartData of type {0} is not supported.", data.GetType().Name));
        }
    }
}