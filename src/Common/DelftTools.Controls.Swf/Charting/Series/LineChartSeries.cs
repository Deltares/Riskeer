using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using DelftTools.Utils;
using Steema.TeeChart.Styles;

namespace DelftTools.Controls.Swf.Charting.Series
{
    /// <summary>
    /// LineChartSeries is represented by a line in a chart.
    /// </summary>
    public class LineChartSeries : ChartSeries, ILineChartSeries
    {
        private readonly Line lineSeries;
        private InterpolationType interpolationType;

        public LineChartSeries() : base(new Line())
        {
            lineSeries = (Line) series;

            lineSeries.ColorEachLine = true;
            lineSeries.ClickableLine = true;

            lineSeries.GetSeriesMark += (s, e) => e.MarkText = Title;
        }

        public LineChartSeries(IChartSeries chartSeries) : this()
        {
            CopySettings(chartSeries);
        }

        public override Color Color
        {
            get
            {
                return lineSeries.Color;
            }
            set
            {
                lineSeries.Color = value;
            }
        }

        public int Width
        {
            get
            {
                return lineSeries.LinePen.Width;
            }
            set
            {
                lineSeries.LinePen.Width = MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize);
            }
        }

        public DashStyle DashStyle
        {
            get
            {
                return lineSeries.LinePen.Style;
            }
            set
            {
                lineSeries.LinePen.Style = value;
            }
        }

        public float[] DashPattern
        {
            get
            {
                return lineSeries.LinePen.DashPattern;
            }
            set
            {
                lineSeries.LinePen.DashPattern = value;
            }
        }

        public Color PointerColor
        {
            get
            {
                return lineSeries.Pointer.Brush.Color;
            }
            set
            {
                lineSeries.Pointer.Brush.Color = value;
            }
        }

        public bool PointerVisible
        {
            get
            {
                return lineSeries.Pointer.Visible;
            }
            set
            {
                lineSeries.Pointer.Visible = value;
            }
        }

        public int PointerSize
        {
            get
            {
                return lineSeries.Pointer.VertSize;
            }
            set
            {
                // just keep it square at this moment.
                lineSeries.Pointer.VertSize = MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize);
                lineSeries.Pointer.HorizSize = MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize);
            }
        }

        public PointerStyles PointerStyle
        {
            get
            {
                string enumName = Enum.GetName(typeof(Steema.TeeChart.Styles.PointerStyles), lineSeries.Pointer.Style);
                return (PointerStyles) Enum.Parse(typeof(PointerStyles), enumName);
            }
            set
            {
                string enumName = Enum.GetName(typeof(PointerStyles), value);
                lineSeries.Pointer.Style =
                    (Steema.TeeChart.Styles.PointerStyles)
                    Enum.Parse(typeof(Steema.TeeChart.Styles.PointerStyles), enumName);
            }
        }

        public ISeriesValueList XValues
        {
            get
            {
                return new SeriesValueList(lineSeries.XValues);
            }
        }

        public ISeriesValueList YValues
        {
            get
            {
                return new SeriesValueList(lineSeries.YValues);
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
                lineSeries.Stairs = (value == InterpolationType.Constant);
            }
        }

        public bool TitleLabelVisible
        {
            get
            {
                return lineSeries.Marks.Visible;
            }
            set
            {
                lineSeries.Marks.Visible = value;
                lineSeries.Marks.Text = Title;
            }
        }

        public bool XValuesDateTime
        {
            get
            {
                return lineSeries.XValues.DateTime;
            }
            set
            {
                lineSeries.XValues.DateTime = value;
            }
        }

        public Color PointerLineColor
        {
            get
            {
                return lineSeries.Pointer.Pen.Color;
            }
            set
            {
                lineSeries.Pointer.Pen.Color = value;
            }
        }

        public bool PointerLineVisible
        {
            get
            {
                return lineSeries.Pointer.Pen.Visible;
            }
            set
            {
                lineSeries.Pointer.Pen.Visible = value;
            }
        }

        public int Transparency
        {
            get
            {
                return lineSeries.Transparency;
            }
            set
            {
                lineSeries.Transparency = value;
            }
        }

        public double MaxYValue()
        {
            return lineSeries.MaxYValue();
        }

        public double MinYValue()
        {
            return lineSeries.MinYValue();
        }

        public double CalcXPos(int index)
        {
            return lineSeries.CalcXPos(index);
        }

        public double CalcYPos(int index)
        {
            return lineSeries.CalcYPos(index);
        }
    }
}