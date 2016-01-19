using System;
using System.Linq;
using Core.Components.Charting.Data;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    public class PointDataConverter : ChartDataConverter
    {
        protected override Type SupportedType
        {
            get
            {
                return typeof(PointData);
            }
        }

        internal override Series Convert(ChartData data)
        {
            var series = new LineSeries
            {
                ItemsSource = data.Points.ToArray(),
                Mapping = TupleToDataPoint,
                LineStyle = LineStyle.None,
                MarkerType = MarkerType.Circle
            };
            return series;
        }
    }
}