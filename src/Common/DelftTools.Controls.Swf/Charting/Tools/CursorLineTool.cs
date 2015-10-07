using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Steema.TeeChart;
using Steema.TeeChart.Tools;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    /// <summary>
    /// Cursor tool adds DropEvent to CursorTool
    /// </summary>
    internal class CursorLineTool : CursorTool, ICursorLineTool
    {
        private bool DragMode { get; set; }
        private Control ChartControl { get; set; }
        private ToolTip ToolTip { get; set; }
        public bool ToolTipEnabled { get; set; }

        /// <summary>
        /// Constructor with default Vertical style
        /// </summary>
        /// <param name="chartControl"></param>
        public CursorLineTool(Control chartControl)
            : this(chartControl, CursorToolStyles.Vertical)
        {
            ToolTipEnabled = true;
        }

        public CursorLineTool(Control chartControl, CursorToolStyles style) : this(chartControl, style, Color.DarkRed, 2, DashStyle.Dash)
        {
        }

        /// <summary>
        /// Constructor with style parameter
        /// </summary>
        /// <param name="chartControl"></param>
        /// <param name="style"></param>
        /// <param name="lineColor"></param>
        /// <param name="lineWidth"></param>
        /// <param name="lineDashStyle"></param>
        public CursorLineTool(Control chartControl, CursorToolStyles style, Color lineColor, int lineWidth, DashStyle lineDashStyle)
        {
            DragMode = false;
            Style = style;
            Pen.Color = lineColor;
            Pen.Width = lineWidth;
            Pen.Style = lineDashStyle;

            ChartControl = chartControl;
            ToolTip = new ToolTip { ShowAlways = false };

            //event listeners
            Change += Cursor_Change;

            //if you create a new cursor tool and have xvalue on 0, the cursor is rendered in the center of the chart (typically not 0). Setting
            //xvalue value to 0 explicitly has no effect (it checks if values are equal, if true, it does not update).
            //HACK: force explicit value
        }


        //events
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<EventArgs> Drop;
        public event EventHandler<EventArgs> ValueChanged;


        /// <summary>
        /// Handles mouse events for the tools and chart of Teechart. Teechart uses a special 
        /// mechanism to let tools cooperate via chart.CancelMouse. Therefor do not use 
        /// control mouse events directly.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="e"></param>
        /// <param name="c"></param>
        protected override void MouseEvent(MouseEventKinds kind, MouseEventArgs e, ref Cursor c)
        {
            if (!Enabled)
                return;
            base.MouseEvent(kind, e, ref c);
            switch (kind)
            {
                case MouseEventKinds.Down:
                    tChart_MouseDown(e);
                    break;
                case MouseEventKinds.Up:
                    tChart_MouseUp(e);
                    break;
                default:
                    {
                        Point P = new Point(e.X, e.Y);
                        CursorClicked cursorClicked = Clicked(P.X, P.Y);
                        if (CursorClicked.None != cursorClicked)
                        {
                            Chart.CancelMouse = true;
                        }
                    }
                    break;
            }
        }

        private void tChart_MouseDown(MouseEventArgs e)
        {
            if (Clicked(e.X, e.Y) == CursorClicked.None)
                return;
            DragMode = true;
            Chart.CancelMouse = true;
            if (MouseDown != null)
                MouseDown(this, e);
        }

        private void tChart_MouseUp(MouseEventArgs e)
        {
            if (!DragMode)
                return;
            Chart.CancelMouse = true;
            if (Drop != null)
            {
                Drop(this, new EventArgs());
            }
            DragMode = false;
            ToolTip.ShowAlways = false;
            ToolTip.Hide(ChartControl);
            if (MouseUp != null)
                MouseUp(this, e);
        }

        private void Cursor_Change(object sender, CursorChangeEventArgs e)
        {
            if (ToolTipEnabled)
            {
                if (Style == CursorToolStyles.Vertical)
                {
                    ToolTip.Show(string.Format("    {0:N2}", XValue), ChartControl);
                }
                else
                {
                    ToolTip.Show(string.Format("    {0:N2}", YValue), ChartControl);
                }
            }
            if (ValueChanged != null)
            {
                ValueChanged(this, new EventArgs());
            }
        }

        public override double XValue
        {
            get
            {
                return base.XValue;
            }
            set
            {                
                //HACK1: force internal change
                if (base.XValue == value)
                {
                    base.XValue = value + 1.0;
                }

                base.XValue = value;

                if (base.XValue != value) 
                {
                    base.XValue = value; //HACK2: force change (second time's the charm)
                }
            }
        }

        /// <summary>
        /// TOOLS-1158 do draw marker lines outside chart area.
        /// Seems a little odd that it is necessary.
        /// </summary>
        /// <param name="e"></param>
        protected override void ChartEvent(EventArgs e)
        {
            if (!(e is AfterDrawEventArgs))
                return;
            Chart.Graphics3D.ClipCube(Chart.ChartRect, 0, 0);
            base.ChartEvent(e);
            Chart.Graphics3D.UnClip();
        }

        public IChartView ChartView { get; set; }

        private bool enabled = true;

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
            }
        }

        public new bool Active
        {
            get { return base.Active; }
            set
            {
                base.Active = value;
                if (ActiveChanged != null)
                {
                    ActiveChanged(this, null);
                }
            }
        }

        public event EventHandler<EventArgs> ActiveChanged;

        #region properties
        public Color LinePenColor
        {
            get { return Pen.Color; }
            set { Pen.Color = value; }
        }
        public float[] LinePenDashPattern
        {
            get { return Pen.DashPattern; }
            set { Pen.DashPattern = value; }
        }
        public int LinePenWidth
        {
            get { return Pen.Width; }
            set { Pen.Width = value; }
        }
        public DashStyle LinePenStyle
        {
            get { return Pen.Style; }
            set { Pen.Style = value; }
        }
        #endregion
    }
}