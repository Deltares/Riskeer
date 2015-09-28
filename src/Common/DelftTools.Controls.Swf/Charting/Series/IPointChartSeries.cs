using System.Drawing;

namespace DelftTools.Controls.Swf.Charting.Series
{
    public interface IPointChartSeries : IChartSeries
    {
        /// <summary>
        /// Size of the points
        /// </summary>
        int Size { get; set; }
        
        /// <summary>
        /// Figure (style) to use for the points
        /// </summary>
        PointerStyles Style { get; set; }

        /// <summary>
        /// Show a line around the points
        /// </summary>
        bool LineVisible { get; set; }

        /// <summary>
        /// Color of the line around the points
        /// </summary>
        Color LineColor { get; set; }
    }
}