using System;

namespace DelftTools.Controls.Swf.Charting
{
    public interface IChartViewTool
    {
        /// <summary>
        /// Event that is fired when the tools active state changes.
        /// </summary>
        event EventHandler<EventArgs> ActiveChanged;

        /// <summary>
        /// Gets or sets the ChartView 
        /// </summary>
        IChartView ChartView { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the tool is active or not
        /// </summary>
        bool Active { get; set; }

        /// <summary>
        /// If the tool is disabled it will not response to mouse event but will stil be visible.
        /// The default Teechart Active property will not draw deactivated tools
        /// </summary>
        bool Enabled { get; set; }
    }
}