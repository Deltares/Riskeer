using System;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Charting.Customized;
using DelftTools.Controls.Swf.Charting.Series;
using Steema.TeeChart;
using Steema.TeeChart.Tools;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    /// <summary>
    /// MarkerTool is a leftbank/rightbank etc. tool for a cross-section
    /// </summary>
    internal class MarkerTool : ToolSeries, IMarkerTool
    {
        private readonly CursorLineTool bottomLine;

        private readonly DeltaShellTChart chartControl;
        
        /// <summary>
        /// Series the markertool is connected to. ex. cross-section
        /// </summary>
        public ILineChartSeries BoundedSeries { get; set; }

        /// <summary>
        /// BottomLine
        /// </summary>
        public ICursorLineTool BottomLine
        {
            get { return bottomLine;  }
        }

        /// <summary>
        /// Constructor of MarkerTool
        /// </summary>
        public MarkerTool(DeltaShellTChart chart)
        {
            chartControl = chart;
            bottomLine = new CursorLineTool(chartControl, CursorToolStyles.Horizontal);
            chartControl.Tools.Add((CursorLineTool)BottomLine);

            //event handlers
            BottomLine.Drop += BottomLineDrop;
        }

        public void Hide(bool hide)
        {
            if (hide)
            {
                if (chartControl.Tools.IndexOf((CursorLineTool)BottomLine) != -1)
                {
                    chartControl.Tools.Remove((CursorLineTool) BottomLine);
                }
            }
            else
            {
                if (chartControl.Tools.IndexOf((CursorLineTool)BottomLine) == -1)
                {
                    chartControl.Tools.Add((CursorLineTool)BottomLine);
                }
            }
        }

        /// <summary>
        /// Enable/Disable the indexed Tool
        /// </summary>
        public new bool Active
        {
            get
            {
                return base.Active;
            }
            set
            {
                base.Active = value;
                BottomLine.Active = value;
                if (ActiveChanged != null)
                {
                    ActiveChanged(this, null);
                }
            }
        }

        public IChartView ChartView { get; set; }

        public event EventHandler<EventArgs> ValueChanged;
        public event EventHandler<MouseEventArgs> MouseDown;

        #region eventHandlers

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
            if (kind == MouseEventKinds.Down)
            {
                ChartMouseDown(null, e);
            }
        }

        private void ChartMouseDown(object sender, MouseEventArgs e)
        {
            if (bottomLine.Clicked(e.X, e.Y) == CursorClicked.None)
            {
                return;
            }
            if (MouseDown != null)
            {
                MouseDown(BottomLine, e);
            }
        }

        private void BottomLineDrop(object sender, EventArgs e)
        {
            ValidateBottomLine();
        }

        private bool enabled = true;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                bottomLine.Enabled = enabled;
            }
        }

        public event EventHandler<EventArgs> ActiveChanged;

        #endregion

        #region private stuff


        private void ValidateBottomLine()
        {
            if (BoundedSeries != null)
            {
                if (BottomLine.YValue > BoundedSeries.MaxYValue())
                {
                    BottomLine.YValue = BoundedSeries.MaxYValue();
                }
                if (BottomLine.YValue < BoundedSeries.MinYValue())
                {
                    BottomLine.YValue = BoundedSeries.MinYValue();
                }
            }

            if(ValueChanged != null)
            {
                ValueChanged(BottomLine, new EventArgs());
            }
        }

        #endregion
    }
}