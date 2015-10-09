using System;
using DelftTools.Controls.Swf.Charting.Series;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public interface IEditPointTool : IChartViewTool
    {
        event EventHandler<HoverPointEventArgs> MouseHoverPoint;
        event EventHandler<PointEventArgs> BeforeDrag;
        event EventHandler<PointEventArgs> AfterPointEdit;
        bool IsPolygon { get; set; }
        bool ClipXValues { get; set; }
        bool ClipYValues { get; set; }
        DragStyle DragStyles { get; set; }
        ILineChartSeries Series { get; set; }
    }
}