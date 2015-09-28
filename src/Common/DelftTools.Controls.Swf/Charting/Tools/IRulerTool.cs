using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public interface IRulerTool : IChartViewTool
    {
        /// <summary>
        /// Width of the line
        /// </summary>
        int LineWidth { get; set; }

        /// <summary>
        /// Dash style of the line
        /// </summary>
        DashStyle DashStyle { get; set; }

        /// <summary>
        /// Pointer color
        /// </summary>
        Color LineColor { get; set; }

        /// <summary>
        /// Cursor when tool is active
        /// </summary>
        Cursor Cursor { get; set; }

        /// <summary>
        /// Allows for customized tooltip. First input argument is the horizontal difference, 
        /// second input argument is the vertical difference. Output argument should be the desired tooltip string.
        /// </summary>
        Func<object, object, string> DifferenceToString { get; set; }

        /// <summary>
        /// Cancels the current operation
        /// </summary>
        void Cancel();
    }
}