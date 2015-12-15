using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

using Core.Common.Controls.Swf.Charting.Series;
using Core.Common.Utils.Collections.Generic;

namespace Core.Common.Controls.Swf.Charting
{
    public interface IChart 
    {
        /// <summary>
        /// Color to use as background color for the chart
        /// </summary>
        Color BackGroundColor { get; set; }

        /// <summary>
        /// Color to use as background color surrounding the chart (control color)
        /// </summary>
        Color SurroundingBackGroundColor { get; set; }

        /// <summary>
        /// Title to display above the chart
        /// </summary>
        string Title { get; set; }

        Font Font { get; set; }

        /// <summary>
        /// Makes the title above the chart visible
        /// </summary>
        bool TitleVisible { get; set; }

        /// <summary>
        /// Stack all series of the same type on top of each other
        /// </summary>
        bool StackSeries { get; set; }

        /// <summary>
        /// Series that are on the chart
        /// </summary>
        IEnumerable<ChartSeries> Series { get; }

        /// <summary>
        /// Left axis of the chart
        /// </summary>
        IChartAxis LeftAxis { get; }

        /// <summary>
        /// Right axis of the chart
        /// </summary>
        IChartAxis RightAxis { get; }

        /// <summary>
        /// Bottom axis of the chart
        /// </summary>
        IChartAxis BottomAxis { get; }

        /// <summary>
        /// The drawing bounds for the chart
        /// </summary>
        Rectangle ChartBounds { get; }

        /// <summary>
        /// Legend showing the series/values of the chart
        /// </summary>
        IChartLegend Legend { get; }

        /// <summary>
        /// Graphics of the chart (for custom drawing)
        /// </summary>
        ChartGraphics Graphics { get; }

        /// <summary>
        /// Parent control containing this chart
        /// </summary>
        Control ParentControl { get; }

        /// <summary>
        /// Cancels the mouse events
        /// </summary>
        bool CancelMouseEvents { set; }

        /// <summary>
        /// The type of a series may be changed
        /// </summary>
        bool AllowSeriesTypeChange { get; set; }


        int GetIndexOfChartSeries(ChartSeries series);

        /// <summary>
        /// Adds the chart series to the chart.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <remarks>Duplicates are prevented from being added.</remarks>
        void AddChartSeries(ChartSeries series);

        /// <summary>
        /// Inserts the chart series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="index">The index.</param>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="index"/> is
        /// less than 0 or greater than the number of series within the chart.</exception>
        void InsertChartSeries(ChartSeries series, int index);

        /// <summary>
        /// Removes the chart series from the chart.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>True if removal was successful; false otherwise</returns>
        bool RemoveChartSeries(ChartSeries series);

        /// <summary>
        /// Removes all chart series form the chart.
        /// </summary>
        void RemoveAllChartSeries();

        /// <summary>
        /// Opens export dialog
        /// </summary>
        void ExportAsImage(IWin32Window owner);

        /// <summary>
        /// Exports a chart without dialog to the specified location. 
        /// Supported types : pdf, jpg, jpeg, gif, png, tiff, bmp and eps
        /// </summary>
        /// <param name="filename">Name of the file to write.(extension determines file type)</param>
        /// <param name="height">Height of the exported image (optional, not used if value is null or smaller than 1) </param>
        /// <param name="width">Width of the exported image (optional, not used if value is null or smaller than 1) </param>
        /// <exception cref="ArgumentException">When <paramref name="filename"/> is null, doesn't contain an extension or 
        /// extension is not supported</exception>
        void ExportAsImage(string filename, int? width, int? height);

        /// <summary>
        /// Creates a bitmap of the chart
        /// </summary>
        Bitmap Bitmap();
    }
}