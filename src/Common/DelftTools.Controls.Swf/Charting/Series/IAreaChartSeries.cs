using System.Drawing;
using System.Drawing.Drawing2D;

namespace DelftTools.Controls.Swf.Charting.Series
{
    public interface IAreaChartSeries : IChartSeries
    {
        /// <summary>
        /// Style to use for the hatch brush
        /// </summary>
        HatchStyle HatchStyle { get; set; }

        /// <summary>
        /// Color used for drawing with Hatch
        /// </summary>
        Color HatchColor { get; set; }

        /// <summary>
        /// Use hatch brush or solid brush
        /// </summary>
        bool UseHatch { get; set; }

        /// <summary>
        /// Transparency of the series
        /// </summary>
        int Transparency { get; set; }

        /// <summary>
        /// Type of interpolation to use (constant, linear or none)
        /// </summary>
        InterpolationType InterpolationType { get; set; }

        /// <summary>
        /// Color of the line above the area
        /// </summary>
        Color LineColor { get; set; }

        /// <summary>
        /// Width of the line above the area
        /// </summary>
        int LineWidth { get; set; }

        /// <summary>
        /// Visibility of the line above the area
        /// </summary>
        bool LineVisible { get; set; }

        /// <summary>
        /// Pointer color
        /// </summary>
        Color PointerColor { get; set; }

        /// <summary>
        /// Visibility of pointer
        /// </summary>
        bool PointerVisible { get; set; }

        /// <summary>
        /// Size of the points
        /// </summary>
        int PointerSize { get; set; }

        /// <summary>
        /// Figure (style) to use for the points
        /// </summary>
        PointerStyles PointerStyle { get; set; }

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