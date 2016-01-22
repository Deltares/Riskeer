using System.Collections.Generic;
using Core.Components.Charting.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// This class converts <see cref="AreaData"/> into <see cref="AreaSeries"/>.
    /// </summary>
    public class AreaDataConverter : ChartDataConverter<AreaData>
    {
        protected override IList<Series> Convert(AreaData data)
        {
            var series = new AreaSeries
            {
                IsVisible = data.IsVisible,
                Tag = data
            };
            foreach (var p in data.Points)
            {
                series.Points.Add(TupleToDataPoint(p));
            }
            if (series.Points.Count > 0)
            {
                series.Points2.Add(series.Points[0]);
            }
            return new List<Series> { series };
        }
    }
}