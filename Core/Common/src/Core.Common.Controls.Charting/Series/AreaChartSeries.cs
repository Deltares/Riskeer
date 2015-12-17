using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Common.Utils;
using Steema.TeeChart.Styles;

namespace Core.Common.Controls.Charting.Series
{
    public class AreaChartSeries : ChartSeries, IAreaChartSeries
    {
        private readonly Area areaSeries;
        private InterpolationType interpolationType;

        public AreaChartSeries(IChartSeries chartSeries) : this()
        {
            CopySettings(chartSeries);
        }

        public AreaChartSeries() : base(new Area())
        {
            areaSeries = (Area) series;
            areaSeries.AreaLines.Visible = false;
            areaSeries.Pointer.Visible = false;
        }

        public override Color Color
        {
            get
            {
                return areaSeries.Color;
            }
            set
            {
                areaSeries.Color = value;

                // also need to set AreaBrush color because TeeChart will otherwise 
                // throw an error (brush == null)
                areaSeries.AreaBrush.Color = value;
            }
        }

        public HatchStyle HatchStyle
        {
            get
            {
                return areaSeries.AreaBrush.Style;
            }
            set
            {
                areaSeries.AreaBrush.Style = value;
            }
        }

        public Color HatchColor
        {
            get
            {
                return areaSeries.AreaBrush.ForegroundColor;
            }
            set
            {
                areaSeries.AreaBrush.ForegroundColor = value;
            }
        }

        public bool UseHatch
        {
            get
            {
                return !areaSeries.AreaBrush.Solid;
            }
            set
            {
                areaSeries.AreaBrush.Solid = !value;
            }
        }

        public int Transparency
        {
            get
            {
                return areaSeries.Transparency;
            }
            set
            {
                areaSeries.Transparency = value;
            }
        }

        public InterpolationType InterpolationType
        {
            get
            {
                return interpolationType;
            }
            set
            {
                interpolationType = value;
                areaSeries.Stairs = (value == InterpolationType.Constant);
            }
        }

        public Color LineColor
        {
            get
            {
                return areaSeries.LinePen.Color;
            }
            set
            {
                areaSeries.LinePen.Color = value;
            }
        }

        public int LineWidth
        {
            get
            {
                return areaSeries.LinePen.Width;
            }
            set
            {
                areaSeries.LinePen.Width = MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize);
            }
        }

        public bool LineVisible
        {
            get
            {
                return areaSeries.LinePen.Visible;
            }
            set
            {
                areaSeries.LinePen.Visible = value;
            }
        }

        public Color PointerColor
        {
            get
            {
                return areaSeries.Pointer.Brush.Color;
            }
            set
            {
                areaSeries.Pointer.Brush.Color = value;
            }
        }

        public bool PointerVisible
        {
            get
            {
                return areaSeries.Pointer.Visible;
            }
            set
            {
                areaSeries.Pointer.Visible = value;
            }
        }

        public int PointerSize
        {
            get
            {
                return areaSeries.Pointer.VertSize;
            }
            set
            {
                areaSeries.Pointer.VertSize = MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize);
                areaSeries.Pointer.HorizSize = MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize);
            }
        }

        public PointerStyles PointerStyle
        {
            get
            {
                string pointerStyleName = Enum.GetName(typeof(Steema.TeeChart.Styles.PointerStyles), areaSeries.Pointer.Style);
                return (PointerStyles) Enum.Parse(typeof(PointerStyles), pointerStyleName);
            }
            set
            {
                string pointerStyleName = Enum.GetName(typeof(PointerStyles), value);
                areaSeries.Pointer.Style = (Steema.TeeChart.Styles.PointerStyles) Enum.Parse(typeof(Steema.TeeChart.Styles.PointerStyles), pointerStyleName);
            }
        }

        public Color PointerLineColor
        {
            get
            {
                return areaSeries.Pointer.Pen.Color;
            }
            set
            {
                areaSeries.Pointer.Pen.Color = value;
            }
        }

        public bool PointerLineVisible
        {
            get
            {
                return areaSeries.Pointer.Pen.Visible;
            }
            set
            {
                areaSeries.Pointer.Pen.Visible = value;
            }
        }
    }
}