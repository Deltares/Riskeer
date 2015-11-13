using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Swf.Charting.Customized;
using Core.Common.Controls.Swf.Properties;
using log4net;
using Steema.TeeChart.Drawing;

namespace Core.Common.Controls.Swf.Charting.Tools
{
    public class RulerTool : IChartViewTool
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RulerTool));
        private readonly RingtoetsTChart teeChart;
        private string toolTip;
        private Steema.TeeChart.Chart.ChartToolTip annotationToolTip;
        private Point measuringStartPoint;
        private Point measuringEndPoint;
        private bool measuring;
        private object horizontalValueStartPoint;
        private object verticalValueStartPoint;
        private bool active;
        private object verticalValueEndPoint;
        private object horizontalValueEndPoint;
        private bool showToolTip;
        private Point lastShowPoint;
        private Cursor cursor;
        private bool selectToolOldState = true;

        public RulerTool(RingtoetsTChart teeChart)
        {
            var chartView = teeChart.Parent as ChartView;

            this.teeChart = teeChart;
            teeChart.AfterDraw += TeeChartAfterDraw;
            teeChart.MouseWheel += SizeChanged;
            teeChart.MouseDown += MouseDown;
            teeChart.MouseMove += MouseMove;

            if (chartView != null)
            {
                chartView.GraphResized += SizeChanged;
                chartView.ViewPortChanged += SizeChanged;
            }

            LineColor = Color.DarkSlateGray;
            LineWidth = 2;
            DashStyle = DashStyle.Dash;
        }

        private void StartMeasuring(Point point)
        {
            var chartView = teeChart.Parent as ChartView;
            if (chartView == null)
            {
                return;
            }

            var bottomMinValue = chartView.Chart.BottomAxis.Minimum;
            var bottomMaxValue = chartView.Chart.BottomAxis.Maximum;
            var leftMinValue = chartView.Chart.LeftAxis.Minimum;
            var leftMaxValue = chartView.Chart.LeftAxis.Maximum;

            measuringStartPoint = point;

            var chartRect = teeChart.Chart.ChartRect;

            horizontalValueStartPoint = GetAxesValue(bottomMinValue,
                                                     bottomMaxValue,
                                                     chartRect.Left,
                                                     (chartRect.Left + chartRect.Width),
                                                     point.X);
            verticalValueStartPoint = GetAxesValue(leftMinValue,
                                                   leftMaxValue,
                                                   (chartRect.Top + chartRect.Height),
                                                   chartRect.Top,
                                                   point.Y);
            horizontalValueEndPoint = double.NaN;
            verticalValueEndPoint = double.NaN;

            measuringEndPoint = measuringStartPoint;
            RemoveToolTip();
            measuring = true;
        }

        private void StopMeasuring(Point point)
        {
            var chartView = teeChart.Parent as ChartView;
            if (chartView == null)
            {
                return;
            }

            var bottomMinValue = chartView.Chart.BottomAxis.Minimum;
            var bottomMaxValue = chartView.Chart.BottomAxis.Maximum;
            var leftMinValue = chartView.Chart.LeftAxis.Minimum;
            var leftMaxValue = chartView.Chart.LeftAxis.Maximum;

            var chartRect = teeChart.Chart.ChartRect;

            horizontalValueEndPoint = GetAxesValue(bottomMinValue,
                                                   bottomMaxValue,
                                                   chartRect.Left,
                                                   (chartRect.Left + chartRect.Width),
                                                   point.X);
            verticalValueEndPoint = GetAxesValue(leftMinValue,
                                                 leftMaxValue,
                                                 (chartRect.Top + chartRect.Height),
                                                 chartRect.Top,
                                                 point.Y);

            var dx = teeChart.Chart.Axes.Bottom.IsDateTime
                         ? (object)
                           (DateTime.FromOADate((double) horizontalValueEndPoint) -
                            DateTime.FromOADate((double) horizontalValueStartPoint))
                         : (double) horizontalValueEndPoint - (double) horizontalValueStartPoint;
            var dy = teeChart.Chart.Axes.Left.IsDateTime
                         ? (object)
                           (DateTime.FromOADate((double) verticalValueEndPoint) - DateTime.FromOADate((double) verticalValueStartPoint))
                         : (double) verticalValueEndPoint - (double) verticalValueStartPoint;
            toolTip = DifferenceToString != null ? DifferenceToString(dx, dy) : GetDefaultDifferenceString(dx, dy);
            showToolTip = true;

            RemoveToolTip();
            lastShowPoint = new Point(measuringEndPoint.X - 20, measuringEndPoint.Y - 20);
            AddToolTip(point);
            AddLogMessage();

            measuring = false;
        }

        private void TeeChartAfterDraw(object sender, Graphics3D graphics3D)
        {
            if (Active && measuringStartPoint != measuringEndPoint && ChartView.Chart.Series.Any(s => s.Visible))
            {
                if (measuringStartPoint.X == int.MinValue || measuringStartPoint.Y == int.MinValue || measuringEndPoint.X == int.MinValue || measuringEndPoint.Y == int.MinValue)
                {
                    return;
                }
                using (Graphics3D gr = teeChart.Graphics3D)
                {
                    gr.Pen = new ChartPen(teeChart.Chart, LineColor)
                    {
                        Width = LineWidth, Style = DashStyle
                    };
                    gr.ClipRectangle(teeChart.Chart.ChartRect);
                    gr.Line(measuringStartPoint, measuringEndPoint);
                }
            }
        }

        private void AddToolTip(Point point)
        {
            if (!showToolTip)
            {
                return;
            }

            var distanceToLine = LineToPointDistance2D(measuringStartPoint, measuringEndPoint, point, true);
            if (!string.IsNullOrEmpty(toolTip) && distanceToLine < 5 && Distance(point, lastShowPoint) > 5)
            {
                lastShowPoint = point;
                annotationToolTip = teeChart.Chart.ToolTip;
                annotationToolTip.Text = toolTip;
                annotationToolTip.Show();
            }
        }

        private void RemoveToolTip()
        {
            if (annotationToolTip != null)
            {
                annotationToolTip.Hide();
                annotationToolTip = null;
                showToolTip = false;
                toolTip = "";
            }
        }

        private void AddLogMessage()
        {
            Log.Info(toolTip);
        }

        private string GetDefaultDifferenceString(object dx, object dy)
        {
            var dxText = dx is TimeSpan
                             ? ((TimeSpan) dx).TotalSeconds.ToString("0.##", CultureInfo.InvariantCulture) + Resources.RulerTool_GetDefaultDifferenceString__seconds
                             : ((double) dx).ToString("0.##", CultureInfo.InvariantCulture);
            var dyText = dy is TimeSpan
                             ? ((TimeSpan) dy).TotalSeconds.ToString("0.##", CultureInfo.InvariantCulture) + Resources.RulerTool_GetDefaultDifferenceString__seconds
                             : ((double) dy).ToString("0.##", CultureInfo.InvariantCulture);
            return string.Format(Resources.RulerTool_GetDefaultDifferenceString_Difference__2___horizontal___0__2___vertical___1_, dxText, dyText, Environment.NewLine);
        }

        private object GetAxesValue(double minValue, double maxValue, int minPixel, int maxPixel, int pixel)
        {
            return minValue + ((maxValue - minValue)/(maxPixel - minPixel))*(pixel - minPixel);
        }

        private int GetPixelValue(double minValue, double maxValue, int minPixel, int maxPixel, double value)
        {
            return (int) (minPixel + ((maxPixel - minPixel)/(maxValue - minValue))*(value - minValue));
        }

        # region IChartViewTool members

        public IChartView ChartView { get; set; }

        public event EventHandler<EventArgs> ActiveChanged;

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                if (active == value)
                {
                    return;
                }
                active = value;
                SetActive();
                if (ActiveChanged != null)
                {
                    ActiveChanged(this, null);
                }
            }
        }

        private void SetActive()
        {
            measuring = false;
            teeChart.Chart.Zoom.Allow = !active;
            RemoveToolTip();
            teeChart.Chart.Invalidate();

            var selectTool = ChartView.GetTool<SelectPointTool>();
            if (active)
            {
                measuringStartPoint = new Point();
                measuringEndPoint = new Point();
                horizontalValueStartPoint = double.NaN;
                verticalValueStartPoint = double.NaN;
                horizontalValueEndPoint = double.NaN;
                verticalValueEndPoint = double.NaN;
                if (selectTool != null)
                {
                    selectToolOldState = selectTool.Active;
                    selectTool.Active = false;
                }
            }
            else
            {
                if (selectTool != null)
                {
                    selectTool.Active = selectToolOldState;
                }
            }
            teeChart.Cursor = active ? Cursor : Cursors.Default;
        }

        public bool Enabled { get; set; }

        # endregion

        # region IRulerTool members

        public int LineWidth { get; set; }

        public DashStyle DashStyle { get; set; }

        public Color LineColor { get; set; }

        public Cursor Cursor
        {
            get
            {
                return cursor ?? Cursors.Default;
            }
            set
            {
                cursor = value;
                teeChart.Cursor = active ? Cursor : Cursors.Default;
            }
        }

        public Func<object, object, string> DifferenceToString { get; set; }

        public void Cancel()
        {
            Active = false;
        }

        # endregion

        # region event handlers

        private void MouseMove(object sender, MouseEventArgs e)
        {
            var currentPoint = new Point(e.X, e.Y);
            if (Active && measuring)
            {
                // Local copy to prevent compiler warning CS1690 (https://msdn.microsoft.com/en-us/library/x524dkh4.aspx)
                var chartRect = teeChart.Chart.ChartRect;
                if (chartRect.Contains(currentPoint))
                {
                    measuringEndPoint = currentPoint;
                }
                teeChart.Chart.Invalidate();
            }

            AddToolTip(currentPoint);
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            if (Active && Steema.TeeChart.Utils.GetMouseButton(e) == MouseButtons.Left)
            {
                var clickedPoint = new Point(e.X, e.Y);
                // Local copy to prevent compiler warning CS1690 (https://msdn.microsoft.com/en-us/library/x524dkh4.aspx)
                var chartRect = teeChart.Chart.ChartRect;
                if (chartRect.Contains(clickedPoint))
                {
                    if (!measuring)
                    {
                        StartMeasuring(clickedPoint);
                    }
                    else
                    {
                        StopMeasuring(clickedPoint);
                    }
                }
            }
        }

        private void SizeChanged(object sender, EventArgs e)
        {
            var chartView = teeChart.Parent as ChartView;
            if (chartView == null)
            {
                return;
            }

            var bottomMinValue = chartView.Chart.BottomAxis.Minimum;
            var bottomMaxValue = chartView.Chart.BottomAxis.Maximum;
            var leftMinValue = chartView.Chart.LeftAxis.Minimum;
            var leftMaxValue = chartView.Chart.LeftAxis.Maximum;

            var chartRect = teeChart.Chart.ChartRect;

            if (horizontalValueStartPoint != null && !double.IsNaN((double) horizontalValueStartPoint))
            {
                var pixelValueStartX = GetPixelValue(bottomMinValue, bottomMaxValue,
                                                     chartRect.Left,
                                                     (chartRect.Left + chartRect.Width),
                                                     (double) horizontalValueStartPoint);
                var pixelValueStartY = GetPixelValue(leftMinValue, leftMaxValue,
                                                     (chartRect.Top + chartRect.Height),
                                                     chartRect.Top,
                                                     (double) verticalValueStartPoint);
                measuringStartPoint = new Point(pixelValueStartX, pixelValueStartY);
            }
            if (horizontalValueEndPoint != null && !double.IsNaN((double) horizontalValueEndPoint))
            {
                var pixelValueEndX = GetPixelValue(bottomMinValue, bottomMaxValue,
                                                   chartRect.Left,
                                                   (chartRect.Left + chartRect.Width),
                                                   (double) horizontalValueEndPoint);
                var pixelValueEndY = GetPixelValue(leftMinValue, leftMaxValue,
                                                   (chartRect.Top + chartRect.Height),
                                                   chartRect.Top,
                                                   (double) verticalValueEndPoint);
                measuringEndPoint = new Point(pixelValueEndX, pixelValueEndY);
            }
        }

        #endregion

        #region Distace calculation

        //Compute the dot product AB . AC
        private double DotProduct(Point pointA, Point pointB, Point pointC)
        {
            var ab = new double[2];
            var bc = new double[2];
            ab[0] = pointB.X - pointA.X;
            ab[1] = pointB.Y - pointA.Y;
            bc[0] = pointC.X - pointB.X;
            bc[1] = pointC.Y - pointB.Y;

            return ab[0]*bc[0] + ab[1]*bc[1];
        }

        //Compute the cross product AB x AC
        private double CrossProduct(Point pointA, Point pointB, Point pointC)
        {
            var ab = new double[2];
            var ac = new double[2];
            ab[0] = pointB.X - pointA.X;
            ab[1] = pointB.Y - pointA.Y;
            ac[0] = pointC.X - pointA.X;
            ac[1] = pointC.Y - pointA.Y;

            return ab[0]*ac[1] - ab[1]*ac[0];
        }

        //Compute the distance from A to B
        private static double Distance(Point pointA, Point pointB)
        {
            double d1 = pointA.X - pointB.X;
            double d2 = pointA.Y - pointB.Y;

            return Math.Sqrt(d1*d1 + d2*d2);
        }

        //Compute the distance from AB to C
        //if isSegment is true, AB is a segment, not a line.
        private double LineToPointDistance2D(Point pointA, Point pointB, Point pointC, bool isSegment)
        {
            var dist = CrossProduct(pointA, pointB, pointC)/Distance(pointA, pointB);
            if (isSegment)
            {
                if (DotProduct(pointA, pointB, pointC) > 0)
                {
                    return Distance(pointB, pointC);
                }

                if (DotProduct(pointB, pointA, pointC) > 0)
                {
                    return Distance(pointA, pointC);
                }
            }
            return Math.Abs(dist);
        }

        #endregion
    }
}