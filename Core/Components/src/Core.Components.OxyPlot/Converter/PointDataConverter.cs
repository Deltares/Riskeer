using System;
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
    public class PointDataConverter : ChartDataConverter
    {
        protected override Type SupportedType
        {
            get
            {
                return typeof(PointData);
            }
        }

        internal override IList<Series> Convert(ChartData data)
        {
            var pointData = (PointData) data;
            var series = new LineSeries
            {
                ItemsSource = pointData.Points.ToArray(),
                IsVisible = pointData.IsVisible,
                Mapping = TupleToDataPoint,
                LineStyle = LineStyle.None,
                MarkerType = MarkerType.Circle,
                Tag = data,
            };
            return new List<Series> { series };
        }
    }
}