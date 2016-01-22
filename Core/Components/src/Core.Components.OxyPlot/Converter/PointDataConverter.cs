using System.Collections.Generic;
using System.Linq;
using Core.Components.Charting.Data;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// This class converts <see cref="PointData"/> into <see cref="LineSeries"/> with point styling.
    /// </summary>
    public class PointDataConverter : ChartDataConverter<PointData>
    {
        protected override IList<Series> Convert(PointData data)
        {
            var series = new LineSeries
            {
                ItemsSource = data.Points.ToArray(),
                IsVisible = data.IsVisible,
                Mapping = TupleToDataPoint,
                LineStyle = LineStyle.None,
                MarkerType = MarkerType.Circle,
                Tag = data,
            };
            return new List<Series> { series };
        }
    }
}