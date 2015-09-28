using System.Drawing;

namespace DelftTools.Controls.Swf.Charting
{
    public interface IChartAxis
    {
        /// <summary>
        /// Format string for the labels
        /// </summary>
        string LabelsFormat { get; set; }

        /// <summary>
        /// Add labels showing the values of the axis (on true)
        /// </summary>
        bool Labels { get; set; }

        /// <summary>
        /// Font with which the labels are displayed
        /// </summary>
        Font LabelsFont { get; set; }

        /// <summary>
        /// Determines if this axis is visible in chart
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Automatically determine the min and max values using the series
        /// </summary>
        bool Automatic { get; set; }

        /// <summary>
        /// Minimum axis value
        /// </summary>
        double Minimum { get; set; }

        /// <summary>
        /// Maximum axis value
        /// </summary>
        double Maximum { get; set; }

        /// <summary>
        /// Extra margin added to the minimum boundary of the axis in pixels.
        /// </summary>
        int MinimumOffset { get; set; }

        /// <summary>
        /// Extra margin added to the maximum boundary of the axis in pixels.
        /// </summary>
        int MaximumOffset { get; set; }

        /// <summary>
        /// Title of this axis
        /// </summary>
        string Title { get; set; }

        Font TitleFont { get; set; }

        /// <summary>
        /// Determines whether the axis uses a logaritmic scale
        /// </summary>
        bool Logaritmic { get; set; }

        /// <summary>
        /// Returns whether the axis is formatted in DateTime.
        /// </summary>
        bool IsDateTime { get; }

        /// <summary>
        /// This function returns the corresponding value of a Screen position. The Screen position must be between Axis limits.
        /// </summary>
        /// <param name="position">Screen position</param>
        double CalcPosPoint(int position);

        /// <summary>
        /// Returns the coordinate position in pixels corresponding to the "Value" parameter in axis scales
        /// </summary>
        /// <param name="value">Value</param>
        int CalcPosValue(double value);
    }
}