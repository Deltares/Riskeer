using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Charting.Customized;
using DelftTools.Controls.Swf.Charting.Series;
using Steema.TeeChart;
using Steema.TeeChart.Styles;
using Steema.TeeChart.Tools;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    /// <summary>
    /// A tool for History: it sets old series to history layout
    /// </summary>
    internal class HistoryTool : ToolSeries, IHistoryTool
    {
        private DeltaShellTChart tChart;
        private int bufferHistory = 5;
        private List<Line> lstLines = new List<Line>();
        
        private readonly ToolTip toolTip;
        private bool toolTipIsShowing;
        private string shownText = "";
        public bool ShowToolTip { get; set; }

        /// <summary>
        /// Constructor for history tool
        /// Works only for Line series at the moment
        /// </summary>
        public HistoryTool(DeltaShellTChart chart)
            : base(chart.Chart)
        {
            tChart = chart;

            toolTip = new ToolTip {ShowAlways = false, InitialDelay = 0, UseAnimation = false, UseFading = false, AutoPopDelay = 0};
        }

        /// <summary>
        /// Amount of histories in buffer
        /// </summary>
        public int BufferHistory
        {
            get { return bufferHistory; }
            set
            {
                if (value >= 0)
                {
                    bufferHistory = value;
                }
            }
        }

        /// <summary>
        /// Remove all history series
        /// </summary>
        public void ClearHistory()
        {
            while (lstLines.Count > 0)
            {
                tChart.Series.Remove(lstLines[0]);
                lstLines.Remove(lstLines[0]);
            }
        }

        /// <summary>
        /// Add Series (Line) to history
        /// </summary>
        /// <param name="series"></param>
        public void Add(IChartSeries series)
        {
            var line = new LineChartSeries
                           {
                               Title = series.Title,
                               ShowInLegend = series.ShowInLegend
                           };

            var lineSeries = (Line) line.series;

            lineSeries.Brush.Visible = false;
            lineSeries.ColorEachLine = false;
            lineSeries.ClickableLine = false;
            lineSeries.Pointer.Visible = false;

            line.XValues.DataMember = series.XValuesDataMember;
            line.YValues.DataMember = series.YValuesDataMember;

            var dataSource = series.DataSource;

            line.DataSource = dataSource is ICloneable
                                        ? ((ICloneable) dataSource).Clone()
                                        : dataSource;
            
            AddLine(line);
        }

        protected override void MouseEvent(MouseEventKinds kind, MouseEventArgs e, ref Cursor c)
        {
            if (kind == MouseEventKinds.Move)
            {
                OnMouseMove(e);
            }
            base.MouseEvent(kind, e, ref c);
        }

        private void OnMouseMove(MouseEventArgs e)
        {
            if (ShowToolTip)
            {
                var linesAtPoint = new List<string>();

                foreach(var line in lstLines)
                {
                    line.ClickableLine = true;
                    int abovePoint = line.Clicked(e.X, e.Y);
                    line.ClickableLine = false;

                    if (abovePoint > -1)
                    {
                        linesAtPoint.Add(line.Title);
                        continue;
                    }
                }

                if (linesAtPoint.Count > 0)
                {
                    var text = string.Join(", ", linesAtPoint.ToArray());

                    if (text.Length > 100)
                    {
                        text = text.Substring(0, 100) + "...";
                    }

                    if (!toolTipIsShowing || !shownText.Equals(text))
                    {
                        toolTip.Show(text, tChart, e.X, e.Y + 20);
                        toolTipIsShowing = true;
                        shownText = text;
                    }
                }
                else if (toolTipIsShowing)
                {
                    toolTip.Hide(tChart);
                    toolTipIsShowing = false;
                }
            }
        }

        private void AddLine(LineChartSeries line)
        {
            //check if line already exists in history.
            if (lstLines.Any(l => l.DataSource == null || l.DataSource.Equals(line.series.DataSource)))
            {
                return;
            }

            while (lstLines.Count - 2 >= bufferHistory)
            {
                ChartView.Chart.Series.Remove(ChartView.Chart.Series.First(s => s.Title == lstLines[0].Title));
                lstLines.Remove(lstLines[0]);
            }

            ChartView.Chart.Series.Add(line);

            var lineSeries = (Line)line.series;

            lstLines.Add(lineSeries);

            //put series at the bottom
            for (int i = tChart.Series.Count - 1; i > 0;i-- )
            {
                tChart.Series.Exchange(i-1, i);
            }

            //hack: assign line pen again (and other color properties) to redo auto-color of tchart
            lineSeries.LinePen.Width = 2;
            lineSeries.LinePen.Color = Color.DarkGray;
            lineSeries.LinePen.Transparency = 25;
            lineSeries.LinePen.Style = DashStyle.Solid;
            lineSeries.Brush.Visible = false;
            lineSeries.Brush.Color = Color.DarkGray;
            lineSeries.Brush.Transparency = 25;
            lineSeries.Color = Color.DarkGray;
            lineSeries.Transparency = 25;
        }

        public IChartView ChartView { get; set; }

        public bool Enabled
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
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
    }
}