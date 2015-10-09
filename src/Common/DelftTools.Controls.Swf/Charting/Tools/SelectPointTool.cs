using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using log4net;
using Steema.TeeChart;
using Steema.TeeChart.Drawing;
using Steema.TeeChart.Styles;
using Steema.TeeChart.Tools;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    /// <summary>
    ///Describes the possible values of the <see cref="NearestPoint.Style"/> property. 
    /// </summary>
    public enum NearestPointStyles
    {
        /// <summary>
        ///No shape is drawn. 
        /// </summary>
        None,

        /// <summary>
        ///Shape is a circle.
        /// </summary>
        Circle,

        /// <summary>
        ///Shape is a rectangle. 
        /// </summary>
        Rectangle,

        /// <summary>
        ///Shape is a diamond. 
        /// </summary>
        Diamond
    };

    public class SelectPoint
    {
        internal SelectPoint(Steema.TeeChart.Styles.Series series, int pointIndex)
        {
            Series = series;
            PointIndex = pointIndex;
        }

        /// <summary>
        /// The index of the point in the datasource.
        /// </summary>
        public int PointIndex { get; private set; }

        internal Steema.TeeChart.Styles.Series Series { get; private set; }
    }

    /// <summary>
    /// Interactive selection of points of a specific series on the chart.
    /// </summary>
    public class SelectPointTool : ChartViewSeriesToolBase, ISelectPointTool
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private static readonly ILog log = LogManager.GetLogger(typeof(SelectPointTool));
        private readonly IEventedList<SelectPoint> selectedPoints = new EventedList<SelectPoint>();

        private readonly bool dragging = false;
        private bool drawLine = true;
        private bool fullRepaint = true;
        private Point mouseLocation;
        private readonly int Point = -1;

        private Color selectedPointerColor = SystemColors.Highlight;
        private readonly Steema.TeeChart.Styles.PointerStyles selectedPointerStyle = Steema.TeeChart.Styles.PointerStyles.Diamond;

        private int size = 10;
        private NearestPointStyles style = NearestPointStyles.Circle;

        internal SelectPointTool(Steema.TeeChart.Chart c) : base(c)
        {
            Pen.Style = DashStyle.Dot;
            Pen.Color = Color.White;
            selectedPoints.CollectionChanged += SelectedPointsCollectionChanged;
            HandleDelete = true;
        }

        /// <summary>
        /// Gets descriptive text.
        /// </summary>
        public override string Description
        {
            get
            {
                return Texts.NearestTool;
            }
        }

        /// <summary>
        /// Gets detailed descriptive text.
        /// </summary>
        public override string Summary
        {
            get
            {
                return Texts.NearestPointSummary;
            }
        }

        /// <summary>
        /// Draws a temporary line from the mouse coordinates to the nearest point.
        /// </summary>
        public bool DrawLine
        {
            get
            {
                return drawLine;
            }
            set
            {
                SetBooleanProperty(ref drawLine, value);
            }
        }

        /// <summary>
        /// Allows the whole Parent Chart to repainted when true.
        /// </summary>
        public bool FullRepaint
        {
            get
            {
                return fullRepaint;
            }
            set
            {
                SetBooleanProperty(ref fullRepaint, value);
            }
        }

        /// <summary>
        /// Determines whether the selectTool should delete point when DEL is pressed
        /// </summary>
        public bool HandleDelete { get; set; }

        /// <summary>
        /// Color  of the point selected in the chart
        /// </summary>
        public Color SelectedPointerColor
        {
            get
            {
                return selectedPointerColor;
            }
            set
            {
                selectedPointerColor = value;
            }
        }

        /// <summary>
        /// Defines the Size of the NearestTool shape.
        /// </summary>
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                SetIntegerProperty(ref size, value);
            }
        }

        /// <summary>
        /// Sets the shape of the NearestTool.
        /// </summary>
        public NearestPointStyles Style
        {
            get
            {
                return style;
            }
            set
            {
                if (style != value)
                {
                    style = value;
                    Invalidate();
                }
            }
        }

        public Cursor Cursor { get; set; }

        ///<summary>
        /// Add point a pointIndex for series to selectedPoints
        ///</summary>
        ///<param name="series"></param>
        ///<param name="pointIndex"></param>
        public void AddPointAtIndexToSelection(IChartSeries series, int pointIndex)
        {
            var teeChartSeries = ((ChartSeries) series).series;
            if (!selectedPoints.Any(p => p.Series == teeChartSeries && p.PointIndex == pointIndex))
            {
                selectedPoints.Add(new SelectPoint(teeChartSeries, pointIndex));
            }
        }

        public void ClearSelection()
        {
            selectedPoints.Clear();
            SelectedPointIndex = -1;
        }

        protected override void OnCustomSeriesGetPointerStyle(CustomPoint series, GetPointerStyleEventArgs e)
        {
            if (e.ValueIndex == selectedPointIndex)
            {
                e.Color = selectedPointerColor;
                e.Style = selectedPointerStyle;
            }
        }

        protected override void Assign(Tool t)
        {
            base.Assign(t);
            SelectPointTool tmp = t as SelectPointTool;
            tmp.Brush = Brush.Clone() as ChartBrush;
            tmp.DrawLine = DrawLine;
            tmp.FullRepaint = FullRepaint;
            tmp.Pen = Pen.Clone() as ChartPen;
            tmp.Style = Style;
        }

        protected override void ChartEvent(EventArgs e)
        {
            base.ChartEvent(e);
            if (e is AfterDrawEventArgs)
            {
                PaintHint();
            }
        }

        protected override void KeyEvent(KeyEventArgs e)
        {
            base.KeyEvent(e);

            if (HandleDelete && (e.KeyCode == Keys.Delete) && (selectedPointIndex != -1) && (null != iSeries))
            {
                // try to delete point from datasource
                DataTable dataTable = iSeries.DataSource as DataTable;
                if (dataTable != null)
                {
                    dataTable.Rows[selectedPointIndex].Delete();
                }
                else
                {
                    throw new NotImplementedException("Deletion not implemented for this type of datasource.");
                }

                //// delete selected point from the series
                //Series.Delete(selectedPointIndex);
                //selectedPointIndex = -1;
                ////todo make sure event is fired, either from tool or series object.
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            log.Debug("SelectPointTool : Up");
            selectedPoints.Clear();
            AddClickedPoint(e.X, e.Y);
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e, ref Cursor c)
        {
            Point P = new Point(e.X, e.Y);
            if (!dragging)
            {
                Steema.TeeChart.Styles.Series s = ClickedSeries(P);

                if (s != null)
                {
                    int abovePoint = s.Clicked(P);
                    if (abovePoint > -1)
                    {
                        c = Cursor ?? Cursors.Cross;
                        //new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("Steema.TeeChart.Cursors.moveTracker.cur"));
                    }
                    Chart.CancelMouse = true;
                }
                else
                {
                    c = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// Element Pen characteristics.
        /// </summary>
        private ChartPen Pen
        {
            get
            {
                if (pPen == null)
                {
                    pPen = new ChartPen(Chart, Color.Blue);
                }
                return pPen;
            }
            set
            {
                pPen = value;
            }
        }

        /// <summary>
        /// Element Brush characteristics.
        /// </summary>
        private ChartBrush Brush
        {
            get
            {
                return bBrush ?? (bBrush = new ChartBrush(Chart, Color.HotPink, false));
            }
            set
            {
                bBrush = value;
            }
        }

        private void SelectedPointsCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        private void PaintHint()
        {
            if (selectedPoints.Count > 0)
            {
                var brush = new SolidBrush(selectedPointerColor);
                Graphics3D g = Chart.Graphics3D;
                foreach (var point in selectedPoints)
                {
                    //the points might have been deleted (multi threading crap)
                    if (point.PointIndex > point.Series.XValues.Count - 1)
                    {
                        return;
                    }

                    //no selection, don't draw
                    if (point.PointIndex < 0)
                    {
                        continue;
                    }

                    int x = point.Series.CalcXPos(point.PointIndex);
                    int y = point.Series.CalcYPos(point.PointIndex);

                    if (!Chart.ChartRect.Contains(x, y)) //outside chart area
                    {
                        continue;
                    }

                    g.Pen = Pen;
                    g.Brush = Brush;

                    g.FillRectangle(brush, (int) (x - size*0.5), (int) (y - size*0.5), size, size);

/*

                    g.Rectangle(new Rectangle(2, 2, totalWidth - 1, totaHeight - 1));

                    g.FillRectangle(brush, new Rectangle(2, 2, width, height));
                    g.Rectangle(pen, new Rectangle(2, 2, width - 1, height - 1));

                    g.FillRectangle(brush, new Rectangle(totalWidth - width + 2, 2, width, height));
                    g.Rectangle(pen, new Rectangle(totalWidth - width + 2, 2, width - 1, height - 1));

                    g.FillRectangle(brush, new Rectangle(totalWidth - width + 2, totaHeight - height + 2, width, height));
                    g.Rectangle(pen, new Rectangle(totalWidth - width + 2, totaHeight - height + 2, width - 1, height - 1));

                    g.FillRectangle(brush, new Rectangle(2, totaHeight - height + 2, width, height));
                    g.Rectangle(pen, new Rectangle(2, totaHeight - height + 2, width - 1, height - 1));
*/
                }
                brush.Dispose();
                //g.Dispose();
            }
            if ((iSeries != null) && (Point != -1))
            {
                Graphics3D g = Chart.Graphics3D;
                g.Pen = Pen;

                int x = iSeries.CalcXPos(Point);
                int y = iSeries.CalcYPos(Point);

                if (style != NearestPointStyles.None)
                {
                    g.Brush = Brush;
                    Rectangle r = Rectangle.FromLTRB(x - size, y - size, x + size, y + size);
                    switch (style)
                    {
                        case NearestPointStyles.Circle:
                            if (Chart.Aspect.View3D)
                            {
                                g.Ellipse(r, iSeries.StartZ);
                            }
                            else
                            {
                                g.Ellipse(r);
                            }
                            break;

                        case NearestPointStyles.Rectangle:
                        {
                            if (Chart.Aspect.View3D)
                            {
                                g.Rectangle(r, iSeries.StartZ);
                            }
                            else
                            {
                                g.Rectangle(r);
                            }
                        }
                            break;

                        case NearestPointStyles.Diamond:
                        {
                            Point[] P = new Point[4];
                            P[0] = new Point(x, y - size);
                            P[1] = new Point(x + size, y);
                            P[2] = new Point(x, y + size);
                            P[3] = new Point(x - size, y);
                            g.Polygon(iSeries.StartZ, P);
                        }
                            break;
                    }
                }

                if (drawLine)
                {
                    g.Pen.Style = DashStyle.Solid;
                    g.MoveTo(mouseLocation);
                    g.LineTo(x, y);
                }
            }
        }

        private void AddClickedPoint(int x, int y)
        {
            var mouseLocation = new Point(x, y);
            //don't remove this..otherwise firstlastvisibleindex will be -1 so the selecttool doesn't work
            if (iSeries != null)
            {
                //// iSeries.CalcFirstLastVisibleIndex is changed from public to internal and we do not want to modify TeeChart
                MethodInfo calcFirstLastVisibleIndex = iSeries.GetType().GetMethod("CalcFirstLastVisibleIndex", BindingFlags.NonPublic | BindingFlags.Instance);
                calcFirstLastVisibleIndex.Invoke(iSeries, new object[]
                {});
            }

            var clickedSeries = ClickedSeries(mouseLocation);
            if (clickedSeries != null)
            {
                LastSelectedSeries = clickedSeries;

                SelectedPointIndex = TeeChartHelper.GetNearestPoint(clickedSeries, mouseLocation, 4);
                if (SelectedPointIndex > -1)
                {
                    selectedPoints.Add(new SelectPoint(clickedSeries, SelectedPointIndex));
                }
            }
        }
    }
}