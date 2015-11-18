using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.Swf.Charting.Series;
using Core.Common.Controls.Swf.Properties;
using log4net;
using Steema.TeeChart;
using Steema.TeeChart.Styles;
using Steema.TeeChart.Tools;

namespace Core.Common.Controls.Swf.Charting.Tools
{
    /// <summary>
    /// Tool that enables interactive adding of points to a series.
    /// </summary>
    public class AddPointTool : ChartViewSeriesToolBase, IAddPointTool
    {
        public event EventHandler<PointEventArgs> PointAdded;
        private static readonly ILog log = LogManager.GetLogger(typeof(AddPointTool));

        private MouseButtons button = MouseButtons.Left;
        private bool dragging = false;

        /// <summary>
        /// Tool that enables interactive adding of points to a series.
        /// </summary>
        /// <param name="c"></param>
        public AddPointTool(Steema.TeeChart.Chart c) : base(c)
        {
            Cursor = Cursors.Hand; //default
        }

        /// <summary>
        /// Tool that enables interactive adding of points to a series.
        /// </summary>
        public AddPointTool() : this(null) {}

        /// <summary>
        /// Gets descriptive text.
        /// </summary>
        public override string Description
        {
            get
            {
                return "AddPoint";
            }
        }

        /// <summary>
        /// Gets detailed descriptive text.
        /// </summary>
        public override string Summary
        {
            get
            {
                return Texts.DragPointSummary;
            }
        }

        public bool Insert { get; set; }

        /// <summary>
        /// Sets which mousebutton activates DragPoint.
        /// </summary>
        public MouseButtons Button
        {
            get
            {
                return button;
            }
            set
            {
                button = value;
            }
        }

        /// <summary>
        /// Determines the type of DragPoint Cursor displayed.
        /// </summary>
        public Cursor Cursor { get; set; }

        public bool AddOnlyIfOnLine { get; set; }

        public ILineChartSeries Series
        {
            get
            {
                return (ILineChartSeries) base.Series;
            }
            set
            {
                base.Series = value;
            }
        }

        protected override void Assign(Tool t)
        {
            base.Assign(t);
            AddPointTool tmp = t as AddPointTool;
            tmp.Button = Button;
            tmp.Cursor = Cursor;
            //tmp.Style = Style;
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
                        c = Cursor;
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

        protected override void OnMouseUp(MouseEventArgs e)
        {
            dragging = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            //button == Button.Left
            if (Steema.TeeChart.Utils.GetMouseButton(e) == button)
            {
                dragging = false;

                if (LastSelectedSeries == null)
                {
                    foreach (Steema.TeeChart.Styles.Series s in Chart.Series)
                    {
                        if (s.Active)
                        {
                            dragging = (s.Clicked(p) != -1);
                            if (dragging)
                            {
                                LastSelectedSeries = s;
                                break;
                            }
                        }
                    }
                }

                if (LastSelectedSeries != null)
                {
                    {
                        if (AddOnlyIfOnLine && !IsPointOnLine(new Point(e.X, e.Y)))
                        {
                            return;
                        }

                        //check if user clicked near enough to line.
                        //calculate real x value from screen value

                        double x = LastSelectedSeries.XScreenToValue(e.X);

                        //interpolate y value
                        double y = LastSelectedSeries.YScreenToValue(e.Y);

                        //add/insert point to the series
                        string oldOrderName = Enum.GetName(typeof(ValueListOrder), LastSelectedSeries.XValues.Order);

                        LastSelectedSeries.XValues.Order = ValueListOrder.None; // .Ascending;
                        if (x > Chart.Axes.Bottom.MinXValue
                            && x < Chart.Axes.Bottom.MaxXValue
                            && y > Chart.Axes.Left.MinYValue
                            && y < Chart.Axes.Left.MaxYValue
                            )
                        {
                            // y = LinearInterPolation(theSeries.XValues, theSeries.YValues, x);
                            if (Insert)
                            {
                                // do not use nearest point but try to calculate nearest line segment
                                double minDistance = double.MaxValue;
                                int minIndex = -1;
                                for (int i = 1; i < LastSelectedSeries.XValues.Count; i++)
                                {
                                    double dist = LinePointDistance(LastSelectedSeries.XValues[i - 1], LastSelectedSeries.YValues[i - 1],
                                                                    LastSelectedSeries.XValues[i], LastSelectedSeries.YValues[i],
                                                                    x, y);
                                    if (dist < minDistance)
                                    {
                                        minDistance = dist;
                                        minIndex = i;
                                    }
                                }
                                LastSelectedSeries.Add(x, y);
                                if (PointAdded != null)
                                {
                                    PointAdded(this, new PointEventArgs(GetChartSeriesFromInternalSeries(LastSelectedSeries), minIndex, x, y));
                                }
                            }
                            else
                            {
                                int index = LastSelectedSeries.Add(x, y);

                                //notify listeners a point was inserted.

                                if (PointAdded != null)
                                {
                                    PointAdded(this, new PointEventArgs(GetChartSeriesFromInternalSeries(LastSelectedSeries), index, x, y));
                                }
                            }
                        }
                        LastSelectedSeries.XValues.Order = (ValueListOrder) Enum.Parse(typeof(ValueListOrder), oldOrderName);
                    }
                }
            }
        }

        private bool IsPointOnLine(Point clickedPoint)
        {
            Steema.TeeChart.Styles.Series s = ClickedSeries(clickedPoint);

            if (s != null)
            {
                int abovePoint = s.Clicked(clickedPoint);
                if (abovePoint > -1)
                {
                    return true;
                }
            }
            return false;
        }

        // HACK 
        private static double Distance(double x1, double y1, double X2, double Y2)
        {
            return Math.Sqrt((x1 - X2)*(x1 - X2) + (y1 - Y2)*(y1 - Y2));
        }

        private static double CrossProduct(double Ax, double Ay, double Bx, double By,
                                           double cx, double cy)
        {
            return (Bx - Ax)*(cy - Ay) - (By - Ay)*(cx - Ax);
        }

        private static double Dot(double Ax, double Ay, double Bx, double By,
                                  double cx, double cy)
        {
            return (Bx - Ax)*(cx - Bx) + (By - Ay)*(cy - By);
        }

        private static double LinePointDistance(double Ax, double Ay, double Bx, double By,
                                                double cx, double cy)
        {
            var dist = Distance(Ax, Ay, Bx, By);
            if (dist < 0.000001)
            {
                return double.MaxValue;
            }
            dist = CrossProduct(Ax, Ay, Bx, By, cx, cy)/dist;
            // if (isSegment) always true
            var dot1 = Dot(Ax, Ay, Bx, By, cx, cy);
            if (dot1 > 0)
            {
                return Distance(Bx, By, cx, cy);
            }
            var dot2 = Dot(Bx, By, Ax, Ay, cx, cy);
            if (dot2 > 0)
            {
                return Distance(Ax, Ay, cx, cy);
            }
            return Math.Abs(dist);
        }
    }

    public interface IAddPointTool : IChartViewTool
    {
        event EventHandler<PointEventArgs> PointAdded;
        MouseButtons Button { get; set; }
        Cursor Cursor { get; set; }
        bool Insert { get; set; }
        bool AddOnlyIfOnLine { get; set; }
        ILineChartSeries Series { get; set; }
    }
}