using System;
using System.Drawing;
using DelftTools.Utils;
using Steema.TeeChart.Styles;

namespace DelftTools.Controls.Swf.Charting.Series
{
    /// <summary>
    /// Series to display points on a chart
    /// </summary>
    public class PointChartSeries : ChartSeries, IPointChartSeries
    {
        private readonly Points pointSeries;

        public PointChartSeries() : base(new Points())
        {
            pointSeries = (Points) series;
            LineVisible = false;
        }

        public PointChartSeries(IChartSeries chartSeries) : this()
        {
            CopySettings(chartSeries);
        }

        public override Color Color
        {
            get
            {
                return pointSeries.Pointer.Brush.Color;
            }
            set
            {
                pointSeries.Pointer.Brush.Color = value;
            }
        }

        public int Size
        {
            get
            {
                return pointSeries.Pointer.VertSize;
            }
            set
            {
                // just keep it square at this moment.
                pointSeries.Pointer.VertSize = MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize);
                pointSeries.Pointer.HorizSize = MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize);
            }
        }

        public PointerStyles Style
        {
            get
            {
                string enumName = Enum.GetName(typeof(Steema.TeeChart.Styles.PointerStyles), pointSeries.Pointer.Style);
                return (PointerStyles) Enum.Parse(typeof(PointerStyles), enumName);
            }
            set
            {
                string enumName = Enum.GetName(typeof(PointerStyles), value);
                pointSeries.Pointer.Style = (Steema.TeeChart.Styles.PointerStyles) Enum.Parse(typeof(Steema.TeeChart.Styles.PointerStyles), enumName);
            }
        }

        public bool LineVisible
        {
            get
            {
                return pointSeries.Pointer.Pen.Visible;
            }
            set
            {
                pointSeries.Pointer.Pen.Visible = value;
            }
        }

        public Color LineColor
        {
            get
            {
                return pointSeries.Pointer.Pen.Color;
            }
            set
            {
                pointSeries.Pointer.Pen.Color = value;
            }
        }
    }
}