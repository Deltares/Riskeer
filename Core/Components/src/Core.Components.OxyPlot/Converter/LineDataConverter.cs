using System.Collections.Generic;
using System.Linq;
using Core.Components.Charting.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// This class converts <see cref="LineData"/> into <see cref="LineSeries"/>.
    /// </summary>
    public class LineDataConverter : ChartDataConverter<LineData>
    {
        protected override IList<Series> Convert(LineData data)
        {
            var series = new LineSeries
            {
                ItemsSource = data.Points.ToArray(),
                Mapping = TupleToDataPoint,
                IsVisible = data.IsVisible,
                Tag = data
            };
            return new List<Series>{ series };
        }
    }
}