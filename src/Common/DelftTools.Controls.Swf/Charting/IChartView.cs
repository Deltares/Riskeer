using System;
using DelftTools.Utils.Collections.Generic;

namespace DelftTools.Controls.Swf.Charting
{
    /// <summary>
    /// 
    /// </summary>
    public interface IChartView : IView
    {
        ///<summary>
        /// Get or set the title of the Chart (shown above the chart)
        ///</summary>
        string Title { get; set; }

        /// <summary>
        /// A collection of tools
        /// </summary>
        IEventedList<IChartViewTool> Tools { get; }

        /// <summary>
        /// Selected point of the active series has been changed
        /// </summary>
        event EventHandler SelectionPointChanged;

        /// <summary>
        /// The visible viewport of the chart has changed either due to a zoom, pan or scroll event
        /// </summary>
        event EventHandler ViewPortChanged;

        /// <summary>
        /// Fires when the active state of one of the tools changes
        /// </summary>
        event EventHandler<EventArgs> ToolsActiveChanged;

        /// <summary>
        /// Set and get the selected Point Index (of the active series)
        /// </summary>
        int SelectedPointIndex { get; set; }

        /// <summary>
        /// Sets the bottom axis to the supplied <param name="min"/> and <param name="max"/> value
        /// </summary>
        /// <param name="min">Min value to set for the bottom axis</param>
        /// <param name="max">Max value to set for the bottom axis</param>
        void ZoomToValues(DateTime min, DateTime max);

        /// <summary>
        /// Sets the bottom axis to the supplied <param name="min"/> and <param name="max"/> value
        /// </summary>
        /// <param name="min">Min value to set for the bottom axis</param>
        /// <param name="max">Max value to set for the bottom axis</param>
        void ZoomToValues(double min, double max);

        /// <summary>
        /// Chart used for this chartView
        /// </summary>
        IChart Chart { get; set; }

        /// <summary>
        /// Zoom and Pan move to chart or tools?
        /// </summary>
        IChartViewZoom Zoom { get; }

        bool AllowPanning { get; set; }

        /// <summary>
        /// Gets the first tool of the right type (<typeparam name="T"/>)
        /// </summary>
        /// <typeparam name="T">Type of tool to search for</typeparam>
        IChartViewTool GetTool<T>();

        /// <summary>
        /// Enables zoom using mouse wheel
        /// </summary>
        bool WheelZoom { get; set; }

        /// <summary>
        /// Disables the deleting of points by SelectPointTool (& delete key)
        /// </summary>
        /// <param name="enable">Enable deleting</param>
        void EnableDelete(bool enable);

        IChartCoordinateService ChartCoordinateService { get; }

        /// <summary>
        /// Exports the chart as image
        /// TODO: just make it return current chart as Image, implement dialogs externally in the same way.
        /// </summary>
        void ExportAsImage();
    }
}