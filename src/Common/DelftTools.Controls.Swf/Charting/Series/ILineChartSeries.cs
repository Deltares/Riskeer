using System.Drawing;
using System.Drawing.Drawing2D;

namespace DelftTools.Controls.Swf.Charting.Series
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
        /// Custom dash pattern (used when <see cref="DashStyle"/> is Custom)
        /// </summary>
        float[] DashPattern { get; set; }

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
        /// Values used for the x axis
        /// </summary>
        ISeriesValueList XValues { get; }

        /// <summary>
        /// Values used for the y axis
        /// </summary>
        ISeriesValueList YValues { get; }

        /// <summary>
        /// Type of interpolation to use (constant, linear or none)
        /// </summary>
        InterpolationType InterpolationType { get; set; }

        /// <summary>
        /// Show title of the series as label
        /// </summary>
        bool TitleLabelVisible { get; set; }
        
        /// <summary>
        /// Values on the x axis should be treated as DateTime values
        /// </summary>
        bool XValuesDateTime { get; set; }

        /// <summary>
        /// Color of the line around the points
        /// </summary>
        Color PointerLineColor { get; set; }

        /// <summary>
        /// Show a line around the points
        /// </summary>
        bool PointerLineVisible { get; set; }

        /// <summary>
        /// Transparency (in %) of the line
        /// </summary>
        int Transparency { get; set; }

        /// <summary>
        /// Gets the maximum value on the y axis
        /// </summary>
        double MaxYValue();

        /// <summary>
        /// Gets the minimum value on the y axis
        /// </summary>
        double MinYValue();

        /// <summary>
        /// Gives horizontal screen position for a given point
        /// </summary>
        /// <param name="index">Index of the point</param>
        double CalcXPos(int index);

        /// <summary>
        /// Gives vertical screen position for a given point
        /// </summary>
        /// <param name="index">Index of the point</param>
        double CalcYPos(int index);
    }
}