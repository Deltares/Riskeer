using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Common.Utils;
using Steema.TeeChart.Styles;

namespace Core.Common.Controls.Swf.Charting.Series
{
    public class BarSeries : ChartSeries
    {
        private readonly Bar barSeries;

        public BarSeries() : base(new Bar())
        {
            barSeries = (Bar) series;
            barSeries.Marks.Visible = false;
            barSeries.MultiBar = MultiBars.None;
            barSeries.BarWidthPercent = 100;
            barSeries.OffsetPercent = 50;
        }

        public BarSeries(IChartSeries chartSeries) : this()
        {
            CopySettings(chartSeries);
        }

        public override Color Color
        {
            get
            {
                return barSeries.Color;
            }
            set
            {
                barSeries.Color = value;
            }
        }

        public DashStyle DashStyle
        {
            get
            {
                return barSeries.Pen.Style;
            }
            set
            {
                barSeries.Pen.Style = value;
            }
        }

        public bool LineVisible
        {
            get
            {
                return barSeries.Pen.Visible;
            }
            set
            {
                barSeries.Pen.Visible = value;
            }
        }

        public int LineWidth
        {
            get
            {
                return barSeries.Pen.Width;
            }
            set
            {
                barSeries.Pen.Width = MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize);
            }
        }

        public Color LineColor
        {
            get
            {
                return barSeries.Pen.Color;
            }
            set
            {
                barSeries.Pen.Color = value;
            }
        }
    }
}