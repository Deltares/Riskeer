using System;
using Core.Components.Charting.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// This class converts <see cref="AreaData"/> into <see cref="AreaSeries"/>.
    /// </summary>
    public class AreaDataConverter : ChartDataConverter
    {
        protected override Type SupportedType
        {
            get
            {
                return typeof(AreaData);
            }
        }

        internal override Series Convert(ChartData data)
        {
            var series = new AreaSeries();
            foreach (var p in data.Points)
            {
                series.Points.Add(TupleToDataPoint(p));
            }
            if (series.Points.Count > 0)
            {
                series.Points2.Add(series.Points[0]);
            }
            return series;
        }
    }
}