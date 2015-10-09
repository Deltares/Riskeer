using System;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Charting.Series;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public interface IMarkerTool : IChartViewTool
    {
        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<EventArgs> ValueChanged;
        ILineChartSeries BoundedSeries { get; set; }
        ICursorLineTool BottomLine { get; }

        void Hide(bool hide);
    }
}