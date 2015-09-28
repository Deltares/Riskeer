using System.Drawing;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public interface IImageTool : IChartViewTool
    {
        /// <summary>
        /// Tooltip string that is displayed when the mouse is above the image
        /// </summary>
        string ToolTip { get; set; }

        /// <summary>
        /// Image to display
        /// </summary>
        Image Image { get; set; }

        /// <summary>
        /// Top position of the top edge of the image
        /// </summary>
        int Top { get; set; }

        /// <summary>
        /// Position of the left of the figure
        /// </summary>
        int Left { get; set; }

        /// <summary>
        /// Width used to display the image
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Height used to display the image
        /// </summary>
        int Height { get; set; }
    }
}