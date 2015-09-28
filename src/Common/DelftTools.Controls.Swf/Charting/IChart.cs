using System;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Utils;
using DelftTools.Utils.Collections.Generic;

namespace DelftTools.Controls.Swf.Charting
{
    public interface IChart: INameable
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
        IEventedList<IChartSeries> Series { get; }

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
        /// Opens export dialog
        /// </summary>
        void ExportAsImage();
        
        /// <summary>
        /// Exports a chart without dialog to the specified location. 
        /// Supported types : pdf, jpg, jpeg, gif, png, tiff, bmp and eps
        /// </summary>
        /// <param name="filename">Name of the file to write.(extension determines file type)</param>
        /// <param name="height">Height of the exported image (optional, not used if value is null or smaller than 1) </param>
        /// <param name="width">Width of the exported image (optional, not used if value is null or smaller than 1) </param>
        /// <exception cref="ArgumentException">When <param name="filename"/> is null, doesn't contain an extension or 
        /// extension is not supported</exception>
        void ExportAsImage(string filename, int? width, int? height);

        /// <summary>
        /// Creates a bitmap of the chart
        /// </summary>
        Bitmap Bitmap();

        /// <summary>
        /// The type of a series may be changed
        /// </summary>
        bool AllowSeriesTypeChange { get;  set; }
    }
}