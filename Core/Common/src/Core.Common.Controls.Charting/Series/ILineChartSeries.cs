using System.Drawing;
using System.Drawing.Drawing2D;

namespace Core.Common.Controls.Charting.Series
{
    public interface ILineChartSeries : IChartSeries
    {
        /// <summary>
        /// Width of the line
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Dash style of the line
        /// </summary>
        DashStyle DashStyle { get; set; }

        /// <summary>
        /// Pointer color
        /// </summary>
        Color PointerColor { get; set; }

        /// <summary>
        /// Visibility of pointer
        /// </summary>
        bool PointerVisible { get; set; }

        /// <summary>
        /// Size of the line points
        /// </summary>
        int PointerSize { get; set; }

        /// <summary>
        /// Figure (style) to use for the points
        /// </summary>
        PointerStyles PointerStyle { get; set; }

        /// <summary>
        /// Type of interpolation to use (constant, linear or none)
        /// </summary>
        InterpolationType InterpolationType { get; set; }

        /// <summary>
        /// Show title of the series as label
        /// </summary>
        bool TitleLabelVisible { get; set; }

        /// <summary>
        /// Color of the line around the points
        /// </summary>
        Color PointerLineColor { get; set; }

        /// <summary>
        /// Show a line around the points
        /// </summary>
        bool PointerLineVisible { get; set; }
    }
}