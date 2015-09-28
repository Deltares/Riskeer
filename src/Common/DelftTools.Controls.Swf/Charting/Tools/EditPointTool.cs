using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Charting.Customized;
using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.Utils;
using log4net;
using Steema.TeeChart;
using Steema.TeeChart.Styles;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    /// <summary>
    /// Tools for moving and deleting points (responds to 'DEL'button)
    /// </summary>
    public class EditPointTool : ChartViewSeriesToolBase, IEditPointTool
    {
        private const double clippingTolerance = 1e-4;
        private DragStyle style = DragStyle.Both;
        private bool pointHasMoved;
        private Color selectedPointerColor = Color.LimeGreen;
        private Steema.TeeChart.Styles.PointerStyles selectedPointerStyle = Steema.TeeChart.Styles.PointerStyles.Diamond;
        private readonly ToolTip toolTip;
        private readonly DeltaShellTChart tChart;
        private bool clipXValues = true;
        private bool restoreZoom;
        private static ILog log = LogManager.GetLogger(typeof(EditPointTool));

        /// <summary>
        /// Allows user to interactively change points in a chart
        /// </summary>
        /// <param name="c"></param>
        public EditPointTool(Steema.TeeChart.Chart c): base(c)
        {
            toolTip = new ToolTip {ShowAlways = false};
        }

        /// <summary>
        /// Allows user to interactively change points in a chart
        /// </summary>
        /// <param name="s"></param>
        public EditPointTool(Steema.TeeChart.Styles.Series s): base(s)
        {
        }

        /// <summary>
        /// Allows user to interactively change points in a chart
        /// </summary>
        public EditPointTool(): this(((Steema.TeeChart.Chart)null))
        {
        }

        /// <summary>
        /// Allows user to interactively change points in a chart
        /// Constructor for schowing a toolTip displaying the coordinates in drga mode.
        /// </summary>
        /// <param name="c"></param>
        public EditPointTool(DeltaShellTChart c)
            : base(c.Chart)
        {
            tChart = c;
            toolTip = new ToolTip();
            toolTip.ShowAlways = false;
        }

        /// <summary>
        /// True if series line can represent a polygon, otherwise x-coordinate of every point will be limited by x value of it's neighbours.
        /// </summary>
        public bool IsPolygon { get; set; }

        /// <summary>
        /// Color  of the point selected in the chart
        /// </summary>
        public Color SelectedPointerColor
        {
            get { return selectedPointerColor; }
            set { selectedPointerColor = value; }
        }

        /// <summary>
        /// Clip means that a point cannot be dragged PASSED another in X direction
        /// </summary>
        public bool ClipXValues
        {
            get { return clipXValues; }
            set { clipXValues = value; }
        }

        /// <summary>
        /// Clip means that a point cannot be dragged PASSED another in Y direction
        /// </summary>
        public bool ClipYValues { get; set; }

        public DragStyle DragStyles
        {
            get { return style; }
            set { style = value; }
        }

        public ILineChartSeries Series
        {
            get { return (ILineChartSeries) base.Series; }
            set { base.Series = value; }
        }

        public event EventHandler<PointEventArgs> BeforeDrag;

        public event EventHandler<HoverPointEventArgs> MouseHoverPoint;
        
        public event EventHandler<PointEventArgs> AfterPointEdit;

        protected override void KeyEvent(KeyEventArgs e)
        {
            base.KeyEvent(e);
            if (e.KeyCode == Keys.Delete && selectedPointIndex != -1)
            {
                // try to delete point from datasource so event is fired 
                DataTable dataTable = LastSelectedSeries.DataSource as DataTable;
                
                if (dataTable != null)
                {
                    dataTable.Rows[selectedPointIndex].Delete();
                    return;
                }

                var lineChartSeries = LastSelectedSeries.DataSource as LineChartSeries;
                if (lineChartSeries != null)
                {
                    lineChartSeries.series.Delete(selectedPointIndex);
                    SelectedPointIndex = -1;
                    return;
                }
                throw new NotImplementedException("Deletion not implemented for this type of datasource.");
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            log.Debug("EditPointTool : Down");
            
            Point p = new Point(e.X, e.Y);
            
            //why not e.Button?
            if (Steema.TeeChart.Utils.GetMouseButton(e) != MouseButtons.Left)
            {
                return;
            }

            // in case of mouseclick, select point nearest to mouse
            if (LastSelectedSeries != null)
            {
                var tolerance = Series != null ? Series.PointerSize : 3;
                int index = TeeChartHelper.GetNearestPoint(LastSelectedSeries, p, tolerance);
                if (-1 != index)
                {
                    if (BeforeDrag != null)
                    {
                        var args = new PointEventArgs(GetChartSeriesFromInternalSeries(LastSelectedSeries), index,
                                                      LastSelectedSeries.XValues[index],
                                                      LastSelectedSeries.YValues[index]);

                        BeforeDrag(this, args);

                        if (args.Cancel)
                        {
                            return;
                        }
                    }

                    SelectedPointIndex = index;
                    Chart.CancelMouse = true;
                    restoreZoom = Chart.Zoom.Active;
                    Chart.Zoom.Active = false;
                    Chart.Zoom.Allow = false;
                    pointHasMoved = false;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e, ref Cursor c)
        {
            //slog.Debug("On mouse move");
            var p = new Point(e.X, e.Y);

            Steema.TeeChart.Styles.Series s = ClickedSeries(p);
            if (s != null)
            {
                var tolerance = Series != null ? Series.PointerSize : 3;
                int abovePoint = TeeChartHelper.GetNearestPoint(s, p, tolerance);
                if (abovePoint > -1)
                {
                    c = GetCursorIcon(style);
                    Chart.CancelMouse = true;

                    if (MouseHoverPoint != null)
                    {
                        MouseHoverPoint(this, new HoverPointEventArgs(GetChartSeriesFromInternalSeries(s), abovePoint, 0.0, 0.0, true));
                    }
                }
            }
            else
            {
                c = Cursors.Default;

                if (MouseHoverPoint != null)
                {
                    MouseHoverPoint(this, new HoverPointEventArgs(null, -1, 0.0, 0.0, false));
                }
            }

            if (Steema.TeeChart.Utils.GetMouseButton(e) != MouseButtons.Left)
            {
                return;
            }

            if (SelectedPointIndex > -1)
            {
                if ((style == DragStyle.X) || (style == DragStyle.Both))
                {
                    if (LastSelectedSeries != null)
                        LastSelectedSeries.XValues[SelectedPointIndex] = CalculateXValue(p);
                    pointHasMoved = true;
                }

                if ((style == DragStyle.Y) || (style == DragStyle.Both))
                {
                    if (LastSelectedSeries != null)
                        LastSelectedSeries.YValues[SelectedPointIndex] = CalculateYValue(p);
                    pointHasMoved = true;
                }
                if (IsPolygon)
                {
                    SynchronizeFirstAndLastPointOfPolygon();
                }
                
                if (pointHasMoved)
                    Invalidate();
            }
        }

        private static Cursor GetCursorIcon(DragStyle dragStyle)
        {
            switch (dragStyle)
            {
                case DragStyle.Both:
                    return Cursors.SizeAll;
                case DragStyle.X:
                    return Cursors.SizeWE;
                case DragStyle.Y:
                    return Cursors.SizeNS;
                default:
                    throw new NotImplementedException(String.Format("No cursor assigned for {0}", dragStyle));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            log.Debug("EditPointTool : Up");

            if (Steema.TeeChart.Utils.GetMouseButton(e) != MouseButtons.Left)
            {
                return;
            }

            if (SelectedPointIndex > -1)
            {
                //int selectedIndex = SelectedPointIndex;
                if (pointHasMoved)
                {
                    if (AfterPointEdit != null)
                    {
                        // AfterPointEdit can change underlying and reset the SelectedPointIndex
                        // MouseMove and MouseUp can run out of sync; return the values set by mousemove = what is visible to the user
                        var chartSeries = GetChartSeriesFromInternalSeries(LastSelectedSeries);
                        AfterPointEdit(this, new PointEventArgs(chartSeries, SelectedPointIndex,
                                                                LastSelectedSeries.XValues[selectedPointIndex], LastSelectedSeries.YValues[selectedPointIndex]));
                        if (IsPolygon)
                        {
                            if (0 == SelectedPointIndex)
                            {
                                if (LastSelectedSeries != null)
                                    AfterPointEdit(this, new PointEventArgs(chartSeries, LastSelectedSeries.XValues.Count - 1,
                                                                            LastSelectedSeries.XValues[selectedPointIndex], LastSelectedSeries.YValues[selectedPointIndex]));
                            }
                            else if (LastSelectedSeries != null)
                                if ((LastSelectedSeries.XValues.Count - 1) == SelectedPointIndex)
                                {
                                    AfterPointEdit(this, new PointEventArgs(chartSeries, 0, LastSelectedSeries.XValues[selectedPointIndex], LastSelectedSeries.YValues[selectedPointIndex]));
                                }
                        }
                    }
                    pointHasMoved = false;
                }

                Chart.Zoom.Active = restoreZoom;
                Chart.Zoom.Allow = true;

                //hide tooltip
                if (tChart != null)
                {
                    toolTip.ShowAlways = false;
                    toolTip.Hide(tChart);
                }
                selectedPointIndex = -1;
            }
        }

        protected override void OnCustomSeriesGetPointerStyle(CustomPoint series, GetPointerStyleEventArgs e)
        {
            if (e.ValueIndex == selectedPointIndex)
            {
                e.Color = selectedPointerColor;
                e.Style = selectedPointerStyle;
            }
        }

        private double CalculateXValue(Point P)
        {
            double xValue = LastSelectedSeries.XScreenToValue(P.X);
            //log.DebugFormat("Clicked at point (x): {0}", xValue);
            if (!IsPolygon && ClipXValues)
            {
                if (SelectedPointIndex > 0)
                {
                    //do not allow to horizontally drag before a neigbouring point.
                    double lowerLimit = LastSelectedSeries.XValues[SelectedPointIndex - 1];
                    if (xValue < lowerLimit )
                    {
                        log.DebugFormat("Fixing x value (left limit) {0} => {1}", xValue, lowerLimit + clippingTolerance);
                        xValue = lowerLimit + clippingTolerance;
                    }
                }
                if (SelectedPointIndex < LastSelectedSeries.Count - 1)
                {
                    //do not allow to horizontally drag past a neigbouring point.
                    double upperLimit = LastSelectedSeries.XValues[SelectedPointIndex + 1];
                    if (xValue > upperLimit )
                    {
                        log.DebugFormat("Fixing x value (right limit) {0} => {1}", xValue, upperLimit - clippingTolerance);
                        xValue = upperLimit - clippingTolerance;
                    }
                }
            }
            return xValue;
        }

        private double CalculateYValue(Point P)
        {
            double yValue = LastSelectedSeries.YScreenToValue(P.Y);

            if (!IsPolygon && ClipYValues)
            {
                //do not allow to vertical drag beyond a neigbouring point.

                double current = LastSelectedSeries.YValues[SelectedPointIndex];
                var limits = new List<double>();
                
                if (SelectedPointIndex > 0)
                {
                    limits.Add(LastSelectedSeries.YValues[SelectedPointIndex - 1]);
                }
                if (SelectedPointIndex < LastSelectedSeries.Count - 1)
                {
                    limits.Add(LastSelectedSeries.YValues[SelectedPointIndex + 1]);
                }

                return Clip(yValue, current, clippingTolerance, limits);
            }
            return yValue;
        }

        private static double Clip(double newValue, double oldValue, double margin, IList<double> limits)
        {
            foreach(double limit in limits) //note: should these be ordered?
            {
                if (limit.IsInRange(newValue, oldValue))
                {
                    return oldValue < limit ? limit - margin : limit + margin;
                }
            }

            return newValue;
        }

        private void SynchronizeFirstAndLastPointOfPolygon()
        {
            //make sure that the last point is equal to the first. If we moved the first we should move the last and vice-versa
            if (0 == SelectedPointIndex)
            {
                LastSelectedSeries.XValues[LastSelectedSeries.XValues.Count - 1] = LastSelectedSeries.XValues[0];
                LastSelectedSeries.YValues[LastSelectedSeries.YValues.Count - 1] = LastSelectedSeries.YValues[0];
            }
            else if ((LastSelectedSeries.XValues.Count - 1) == SelectedPointIndex)
            {
                LastSelectedSeries.XValues[0] = LastSelectedSeries.XValues[LastSelectedSeries.XValues.Count - 1];
                LastSelectedSeries.YValues[0] = LastSelectedSeries.YValues[LastSelectedSeries.YValues.Count - 1];
            }

        }
    }

    public enum DragStyle
    {
        Both,
        X,
        Y
    }
}