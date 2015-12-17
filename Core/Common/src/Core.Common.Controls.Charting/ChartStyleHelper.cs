using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.Charting.Series;

namespace Core.Common.Controls.Charting
{
    public static class ChartStyleHelper
    {
        /// <summary>
        /// Copies chart, axis and series styles from a <paramref name="source"/> chart to a <paramref name="target"/> chart. 
        /// To copy styles from series this method uses the Tag property of the ChartSeries to identify series to copy a style to.
        /// </summary>
        /// <param name="source">Source chart</param>
        /// <param name="target">Target chart</param>
        public static void CopyStyle(IChart source, IChart target)
        {
            // First copy chart styles
            CopyChartStyle(source, target);

            // Copy axis styles
            CopyAxisStyle(source.LeftAxis, target.LeftAxis);
            CopyAxisStyle(source.BottomAxis, target.BottomAxis);

            // Copy series styles
            CopySeriesStyles(source.Series, target.Series);
        }

        /// <summary>
        /// Copies only the chart related styles from <paramref name="source"/> to <paramref name="target"/>.
        /// </summary>
        /// <param name="source">Source chart</param>
        /// <param name="target">Target chart</param>
        public static void CopyChartStyle(IChart source, IChart target)
        {
            target.Legend.Visible = source.Legend.Visible;
            target.Legend.Alignment = source.Legend.Alignment;
            target.Legend.Font = source.Legend.Font;
            target.TitleVisible = source.TitleVisible;
            target.Title = source.Title;
            target.Font = source.Font;
            target.StackSeries = source.StackSeries;
            target.SurroundingBackGroundColor = source.SurroundingBackGroundColor;
            target.BackGroundColor = source.BackGroundColor;
            target.AllowSeriesTypeChange = source.AllowSeriesTypeChange;
        }

        /// <summary>
        /// Copies the style of a <paramref name="source"/> axis to a <paramref name="target"/> axis. This includes legend style, label style and axis scaling.
        /// </summary>
        /// <param name="source">Source chart axis</param>
        /// <param name="target">Target chart axis</param>
        public static void CopyAxisStyle(IChartAxis source, IChartAxis target)
        {
            target.Labels = source.Labels;
            target.Automatic = source.Automatic;
            target.Maximum = source.Maximum;
            target.Minimum = source.Minimum;
            target.Title = source.Title;
            target.Visible = source.Visible;
            target.LabelsFont = source.LabelsFont;
            target.TitleFont = source.TitleFont;
            target.LabelsFormat = source.LabelsFormat;
            target.Logaritmic = source.Logaritmic;
        }

        /// <summary>
        /// Copies style of all chart series in the <paramref name="source"/> collection to styles in the <paramref name="target"/> collection. Tag is used to match series (series in the target collection with a tag that is also in one of ther series of the source collection get the style of the first item witht the same Tag in the source collection).
        /// </summary>
        public static void CopySeriesStyles(IEnumerable<IChartSeries> source, IEnumerable<IChartSeries> target)
        {
            foreach (var sourceSeries in source)
            {
                var series = sourceSeries;
                foreach (var targetSeries in target.Where(s => series.Tag == s.Tag))
                {
                    CopyStyle(series, targetSeries);

                    var polygonChartSeries = series as IPolygonChartSeries;
                    if (polygonChartSeries != null)
                    {
                        CopyStyle(polygonChartSeries, (IPolygonChartSeries) targetSeries);
                    }
                    var pointChartSeries = series as IPointChartSeries;
                    if (pointChartSeries != null)
                    {
                        CopyStyle(pointChartSeries, (IPointChartSeries) targetSeries);
                    }
                    var areaChartSeries = series as IAreaChartSeries;
                    if (areaChartSeries != null)
                    {
                        CopyStyle(areaChartSeries, (IAreaChartSeries) targetSeries);
                    }
                    var lineChartSeries = series as ILineChartSeries;
                    if (lineChartSeries != null)
                    {
                        CopyStyle(lineChartSeries, (ILineChartSeries) targetSeries);
                    }
                }
            }
        }

        /// <summary>
        /// Copies the style of one chart series to another.
        /// </summary>
        public static void CopySeriesStyles(IChartSeries sourceSeries, IChartSeries targetSeries)
        {
            CopyStyle(sourceSeries, targetSeries);
            if (sourceSeries is IPointChartSeries && targetSeries is IPointChartSeries)
            {
                CopyStyle((IPointChartSeries) sourceSeries, (IPointChartSeries) targetSeries);
            }
            if (sourceSeries is ILineChartSeries && targetSeries is ILineChartSeries)
            {
                CopyStyle((ILineChartSeries) sourceSeries, (ILineChartSeries) targetSeries);
            }
            if (sourceSeries is IAreaChartSeries && targetSeries is IAreaChartSeries)
            {
                CopyStyle((IAreaChartSeries) sourceSeries, (IAreaChartSeries) targetSeries);
            }
            if (sourceSeries is IPolygonChartSeries && targetSeries is IPolygonChartSeries)
            {
                CopyStyle((IPolygonChartSeries) sourceSeries, (IPolygonChartSeries) targetSeries);
            }
        }

        private static void CopyStyle(IPolygonChartSeries series, IPolygonChartSeries targetSeries)
        {
            targetSeries.HatchStyle = series.HatchStyle;
            targetSeries.HatchColor = series.HatchColor;
            targetSeries.UseHatch = series.UseHatch;
            targetSeries.Transparency = series.Transparency;
            targetSeries.LineColor = series.LineColor;
            targetSeries.LineWidth = series.LineWidth;
            targetSeries.LineVisible = series.LineVisible;
            targetSeries.LineStyle = series.LineStyle;
            targetSeries.AutoClose = series.AutoClose;
        }

        private static void CopyStyle(IAreaChartSeries series, IAreaChartSeries targetSeries)
        {
            targetSeries.Transparency = series.Transparency;
            targetSeries.InterpolationType = series.InterpolationType;
            targetSeries.UseHatch = series.UseHatch;
            targetSeries.HatchStyle = series.HatchStyle;
            targetSeries.HatchColor = series.HatchColor;
            targetSeries.LineColor = series.LineColor;
            targetSeries.LineWidth = series.LineWidth;
            targetSeries.LineVisible = series.LineVisible;
            targetSeries.PointerColor = series.PointerColor;
            targetSeries.PointerStyle = series.PointerStyle;
            targetSeries.PointerVisible = series.PointerVisible;
            targetSeries.PointerSize = series.PointerSize;
            targetSeries.PointerLineColor = series.PointerLineColor;
            targetSeries.PointerLineVisible = series.PointerLineVisible;
        }

        private static void CopyStyle(ILineChartSeries series, ILineChartSeries targetSeries)
        {
            targetSeries.Width = series.Width;
            targetSeries.InterpolationType = series.InterpolationType;
            targetSeries.TitleLabelVisible = series.TitleLabelVisible;
            targetSeries.DashStyle = series.DashStyle;
            targetSeries.PointerSize = series.PointerSize;
            targetSeries.PointerColor = series.PointerColor;
            targetSeries.PointerStyle = series.PointerStyle;
            targetSeries.PointerVisible = series.PointerVisible;
            targetSeries.PointerLineColor = series.PointerLineColor;
            targetSeries.PointerLineVisible = series.PointerLineVisible;
        }

        private static void CopyStyle(IPointChartSeries series, IPointChartSeries targetSeries)
        {
            targetSeries.LineColor = series.LineColor;
            targetSeries.LineVisible = series.LineVisible;
            targetSeries.Size = series.Size;
            targetSeries.Style = series.Style;
        }

        private static void CopyStyle(IChartSeries series, IChartSeries targetSeries)
        {
            targetSeries.ShowInLegend = series.ShowInLegend;
            targetSeries.Title = series.Title;
            targetSeries.Color = series.Color;
            targetSeries.Visible = series.Visible;
            targetSeries.VertAxis = series.VertAxis;
        }
    }
}