using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Swf.Charting.Series;
using Steema.TeeChart;
using Steema.TeeChart.Styles;
using Steema.TeeChart.Tools;

namespace Core.Common.Controls.Swf.Charting.Tools
{
    ///<summary>
    /// Base class for EditPointTool,SelectPointTool and AddPointTool
    ///</summary>
    public abstract class ChartViewSeriesToolBase : ToolSeries, IChartViewSeriesTool
    {
        public event SelectionChangedEventHandler SelectionChanged;

        public event EventHandler<EventArgs> ActiveChanged;

        protected int selectedPointIndex = -1;
        private bool enabled = true;

        private ChartSeries series;

        private Steema.TeeChart.Styles.Series previousSelectedSeries;

        private Steema.TeeChart.Styles.Series lastSelectedSeries;

        protected ChartViewSeriesToolBase(Steema.TeeChart.Chart chart) : base(chart) {}

        protected ChartViewSeriesToolBase(Steema.TeeChart.Styles.Series series) : base(series) {}

        /// <summary>
        /// Selects a point in the chart.
        /// </summary>
        public int SelectedPointIndex
        {
            get
            {
                return selectedPointIndex;
            }
            protected set
            {
                if (selectedPointIndex != value || !ReferenceEquals(previousSelectedSeries, LastSelectedSeries))
                {
                    previousSelectedSeries = LastSelectedSeries;

                    if ((LastSelectedSeries != null))
                    {
                        if ((value > LastSelectedSeries.Count - 1))
                        {
                            throw new ArgumentException("Selected index outside range of series");
                        }

                        selectedPointIndex = value;
                        if (SelectionChanged != null)
                        {
                            if (selectedPointIndex != -1)
                            {
                                SelectionChanged(this,
                                                 new PointEventArgs(
                                                     GetChartSeriesFromInternalSeries(LastSelectedSeries),
                                                     selectedPointIndex,
                                                     LastSelectedSeries.XValues[selectedPointIndex],
                                                     LastSelectedSeries.YValues[selectedPointIndex]));
                            }
                            else
                            {
                                SelectionChanged(this,
                                                 new PointEventArgs(
                                                     GetChartSeriesFromInternalSeries(LastSelectedSeries),
                                                     selectedPointIndex, double.NaN, double.NaN));
                            }
                        }
                        Invalidate();
                    }
                    else
                    {
                        throw new InvalidOperationException("LastSelectedSeries is null!");
                    }
                }
            }
        }

        public IChartView ChartView { get; set; }

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }

        public new bool Active
        {
            get
            {
                return base.Active;
            }
            set
            {
                base.Active = value;
                if (ActiveChanged != null)
                {
                    ActiveChanged(this, null);
                }
            }
        }

        public new IChartSeries Series
        {
            get
            {
                return series;
            }
            set
            {
                series = (ChartSeries) value;
                iSeries = series.series;
            }
        }

        protected Steema.TeeChart.Styles.Series LastSelectedSeries
        {
            get
            {
                return iSeries ?? lastSelectedSeries; //if internal series is set, use that (tool is only for that series), otherwise return last selected    
            }
            set
            {
                if (iSeries != null && iSeries != value)
                {
                    throw new ArgumentException(String.Format("This tool only accepts {0} as series", iSeries));
                }
                lastSelectedSeries = value;
            }
        }

        protected override void SetSeries(Steema.TeeChart.Styles.Series series)
        {
            var customPoint = ((ChartSeries) Series).series as CustomPoint;
            if (customPoint != null)
            {
                customPoint.GetPointerStyle -= OnCustomSeriesGetPointerStyle;
            }

            base.SetSeries(series);

            customPoint = series as CustomPoint;
            if (customPoint != null)
            {
                customPoint.GetPointerStyle += OnCustomSeriesGetPointerStyle;
            }
        }

        protected virtual void OnCustomSeriesGetPointerStyle(CustomPoint customSeries, GetPointerStyleEventArgs e) {}

        protected Steema.TeeChart.Styles.Series ClickedSeries(Point p)
        {
            return ClickedSeries(p.X, p.Y);
        }

        protected Steema.TeeChart.Styles.Series ClickedSeries(int x, int y)
        {
            if (iSeries == null)
            {
                var seriesInRenderOrder = Chart.Series.OfType<Steema.TeeChart.Styles.Series>().Reverse();

                return seriesInRenderOrder.FirstOrDefault(s => (s.Active) && (s.Clicked(x, y) != -1));
            }
            else if (iSeries.Clicked(x, y) != -1)
            {
                return iSeries;
            }

            return null;
        }

        protected IChartSeries GetChartSeriesFromInternalSeries(Steema.TeeChart.Styles.Series internalSeries)
        {
            if (ChartView != null && ChartView.Chart != null)
            {
                var matchingSeries =
                    ChartView.Chart.Series.OfType<ChartSeries>().FirstOrDefault(
                        cs => ReferenceEquals(cs.series, internalSeries));

                if (matchingSeries != null)
                {
                    return matchingSeries;
                }

                matchingSeries =
                    ChartView.Chart.Series.OfType<ChartSeries>().FirstOrDefault(cs => cs.Title == internalSeries.Title);

                if (matchingSeries != null)
                {
                    return matchingSeries;
                }
            }
            throw new ArgumentException("Unknown TeeChart series: not related to any known ChartSeries");
        }

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
            {
                return;
            }
            switch (kind)
            {
                case MouseEventKinds.Up:
                    OnMouseUp(e);
                    break;
                case MouseEventKinds.Move:
                    OnMouseMove(e, ref c);
                    break;
                case MouseEventKinds.Down:
                    OnMouseDown(e);
                    break;
            }
        }

        protected virtual void OnMouseDown(MouseEventArgs mouseEventArgs) {}

        protected virtual void OnMouseMove(MouseEventArgs mouseEventArgs, ref Cursor cursor) {}

        protected virtual void OnMouseUp(MouseEventArgs mouseEventArgs) {}
    }
}