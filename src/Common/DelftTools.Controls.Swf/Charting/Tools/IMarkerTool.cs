using System;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Charting.Series;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public interface IMarkerTool : IChartViewTool
    {
        ILineChartSeries BoundedSeries { get; set; }
        ICursorLineTool BottomLine { get; }

        void Hide(bool hide);

        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<EventArgs> ValueChanged;
    }
}