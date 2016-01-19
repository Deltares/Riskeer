using System;
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

        internal override Series Convert(ChartData data)
        {
            var series = new LineSeries
            {
                ItemsSource = data.Points.ToArray(),
                Mapping = TupleToDataPoint
            };
            return series;
        }
    }
}