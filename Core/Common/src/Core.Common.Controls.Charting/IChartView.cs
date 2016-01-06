using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.Views;

namespace Core.Common.Controls.Charting
{
    /// <summary>
    /// 
    /// </summary>
    public interface IChartView : IView
    {
        IWin32Window Owner { get; set; }

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
        /// Get or set the title of the Chart (shown above the chart)
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// A collection of tools
        /// </summary>
        IEnumerable<IChartViewTool> Tools { get; }

        /// <summary>
        /// Set and get the selected Point Index (of the active series)
        /// </summary>
        int SelectedPointIndex { get; set; }

        /// <summary>
        /// Chart used for this chartView
        /// </summary>
        IChart Chart { get; set; }

        bool AllowPanning { get; set; }

        /// <summary>
        /// Enables zoom using mouse wheel
        /// </summary>
        bool WheelZoom { get; set; }

        ChartCoordinateService ChartCoordinateService { get; }

        /// <summary>
        /// Sets the bottom axis to the supplied <paramref name="min"/> and <paramref name="max"/> value
        /// </summary>
        /// <param name="min">Min value to set for the bottom axis</param>
        /// <param name="max">Max value to set for the bottom axis</param>
        void ZoomToValues(DateTime min, DateTime max);

        /// <summary>
        /// Sets the bottom axis to the supplied <paramref name="min"/> and <paramref name="max"/> value
        /// </summary>
        /// <param name="min">Min value to set for the bottom axis</param>
        /// <param name="max">Max value to set for the bottom axis</param>
        void ZoomToValues(double min, double max);

        /// <summary>
        /// Attempts to find the first tool of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of tool to search for</typeparam>
        T GetTool<T>() where T : IChartViewTool;

        /// <summary>
        /// Disables the deleting of points by SelectPointTool (&amp; delete key)
        /// </summary>
        /// <param name="enable">Enable deleting</param>
        void EnableDelete(bool enable);

        /// <summary>
        /// Exports the chart as image
        /// TODO: just make it return current chart as Image, implement dialogs externally in the same way.
        /// </summary>
        void ExportAsImage();
    }
}