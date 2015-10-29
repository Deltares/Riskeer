using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Common.Controls.Swf.Properties;
using Core.Common.Utils;

namespace Core.Common.Controls.Swf.Charting.Series
{
    public class PolygonChartSeries : ChartSeries, IPolygonChartSeries
    {
        private readonly PolygonSeries polygonSeries;

        public PolygonChartSeries(IChartSeries chartSeries) : this()
        {
            CopySettings(chartSeries);
            ChartStyleHelper.CopySeriesStyles(chartSeries, this);
            Tag = chartSeries.Tag;
        }

        public PolygonChartSeries() : base(new PolygonSeries())
        {
            polygonSeries = (PolygonSeries) series;
            DefaultNullValue = double.NaN;
        }

        public override Color Color
        {
            get
            {
                return series.Color;
            }
            set
            {
                series.Color = value;

                // also need to set AreaBrush color because TeeChart will otherwise 
                // throw an error (brush == null)
                (polygonSeries).bBrush.Color = value;
            }
        }

        public Color LineColor
        {
            get
            {
                return polygonSeries.Pen.Color;
            }
            set
            {
                polygonSeries.Pen.Color = value;
            }
        }

        public int LineWidth
        {
            get
            {
                return polygonSeries.Pen.Width;
            }
            set
            {
                polygonSeries.Pen.Width = MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize);
            }
        }

        public bool LineVisible
        {
            get
            {
                return polygonSeries.Pen.Visible;
            }
            set
            {
                polygonSeries.Pen.Visible = value;
                polygonSeries.Repaint();
            }
        }

        public DashStyle LineStyle
        {
            get
            {
                return polygonSeries.Pen.Style;
            }
            set
            {
                polygonSeries.Pen.Style = value;
            }
        }

        public bool AutoClose
        {
            get
            {
                return polygonSeries.AutoClose;
            }
            set
            {
                polygonSeries.AutoClose = value;
            }
        }

        # region Hatch

        public HatchStyle HatchStyle
        {
            get
            {
                return polygonSeries.bBrush.Style;
            }
            set
            {
                polygonSeries.bBrush.Style = value;
            }
        }

        public Color HatchColor
        {
            get
            {
                return polygonSeries.bBrush.ForegroundColor;
            }
            set
            {
                polygonSeries.bBrush.ForegroundColor = value;
            }
        }

        public bool UseHatch
        {
            get
            {
                return !polygonSeries.bBrush.Solid;
            }
            set
            {
                polygonSeries.bBrush.Solid = !value;
            }
        }

        /// <summary>
        /// Percentage transparancy. This should be between 0 and 100.
        /// </summary>
        public int Transparency
        {
            get
            {
                return polygonSeries.Pen.Transparency;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.PolygonChartSeries_Transparency_Transparancy_should_be_between_0_and_100);
                }
                polygonSeries.Pen.Transparency = value;
                polygonSeries.bBrush.Transparency = value;
            }
        }

        # endregion
    }
}