using System;
using System.Collections.Generic;
using Core.Components.Charting.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// This class converts the <see cref="ChartData"/> in <see cref="ChartDataCollection"/> into 
    /// (one or more) <see cref="Series"/>.
    /// </summary>
    public class ChartDataCollectionConverter : ChartDataConverter
    {
        protected override Type SupportedType
        {
            get
            {
                return typeof(ChartDataCollection);
            }
        }

        internal override IList<Series> Convert(ChartData data)
        {
            var factory = new SeriesFactory();
            var seriesCollection = new List<Series>();
            foreach(var series in ((ChartDataCollection)data).List)
            {
                seriesCollection.AddRange(factory.Create(series));
            }
            return seriesCollection;
        }
    }
}