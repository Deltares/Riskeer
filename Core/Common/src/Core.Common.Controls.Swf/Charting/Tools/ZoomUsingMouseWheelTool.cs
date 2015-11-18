using System.Windows.Forms;
using Steema.TeeChart;
using Steema.TeeChart.Tools;

namespace Core.Common.Controls.Swf.Charting.Tools
{
    /// <summary>
    /// Zooms in / out on mouse wheel.
    /// </summary>
    public class ZoomUsingMouseWheelTool : ToolSeries
    {
        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="chart"></param>
        public ZoomUsingMouseWheelTool(Steema.TeeChart.Chart chart) : base(chart) {}

        /// <summary>
        /// check wether mousewheel is used together with ctrl button
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="e"></param>
        /// <param name="c"></param>
        protected override void MouseEvent(MouseEventKinds kind, MouseEventArgs e, ref Cursor c)
        {
            /*  if(kind==MouseEventKinds.Up)
            {
                chart.Axes.Left.Automatic = true;
                chart.Axes.Bottom.Automatic = true;
            }*/
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control && kind == MouseEventKinds.Wheel)
            {
                //TODO fix zoom behavior: zooming out does not work as expected.
                //this.chart.Zoom.ZoomPercent(100 + e.Delta/20);

                var zoomFraction = (100.0 + e.Delta/20.0)/100.0;

                var xmin = Chart.Axes.Bottom.Minimum;
                var xmax = Chart.Axes.Bottom.Maximum;
                var ymin = Chart.Axes.Left.Minimum;
                var ymax = Chart.Axes.Left.Maximum;

                //center of map
                //var xcenter = xmin + (xmax - xmin) / 2;
                //var ycenter = ymin + (ymax - ymin) / 2;

                ////retrieve series and calculate mouseposition expressed in world coordinates
                //Steema.TeeChart.Styles.Series theSeries = null;
                //foreach (Steema.TeeChart.Styles.Series s in chart.Series)
                //{
                //    if (s.Active)
                //    {
                //            theSeries = s;
                //            break;
                //    }
                //}
                //if (theSeries ==null ) return;

                //var xNewCenter = theSeries.XScreenToValue(e.X);
                //var yNewCenter = theSeries.YScreenToValue(e.Y);

                ////move to new center
                //xmin += xNewCenter - xcenter;
                //xmax += xNewCenter - xcenter;
                //ymin += yNewCenter - ycenter;
                //ymax += yNewCenter - ycenter;

                var d2x = (xmax - xmin)*(1 - 1/zoomFraction);
                var d2y = (ymax - ymin)*(1 - 1/zoomFraction);

                Chart.Axes.Left.SetMinMax(ymin + d2y/2, ymax - d2y/2);
                Chart.Axes.Bottom.SetMinMax(xmin + d2x/2, xmax - d2x/2);

/*
                chart.Axes.Bottom.Minimum = xmin + d2x / 2;
                chart.Axes.Bottom.Maximum = xmax - d2x / 2;
               chart.Axes.Left.Minimum = ymin + d2y / 2;
               chart.Axes.Left.Maximum = ymax - d2y / 2;
*/
            }
        }
    }
}