using System;
using System.Collections.Generic;
using System.Linq;
using Core.Components.Charting.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// This class converts <see cref="LineData"/> into <see cref="LineSeries"/>.
    /// </summary>
    public class LineDataConverter : ChartDataConverter
    {
        protected override Type SupportedType
        {
            get
            {
                return typeof(LineData);
            }
        }

        internal override IList<Series> Convert(ChartData data)
        {
            var lineData = (LineData) data;
            var series = new LineSeries
            {
                ItemsSource = lineData.Points.ToArray(),
                Mapping = TupleToDataPoint,
                IsVisible = lineData.IsVisible,
                Tag = data
            };
            return new List<Series>{ series };
        }
    }
}