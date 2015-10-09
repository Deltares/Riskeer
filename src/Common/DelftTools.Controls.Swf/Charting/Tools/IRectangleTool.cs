namespace DelftTools.Controls.Swf.Charting.Tools
{
    public interface IRectangleTool : IChartViewTool
    {
        event RectangleToolDraggingEventHandler Dragging;
        event RectangleToolResizingEventHandler Resizing;
        event RectangleToolResizedEventHandler Resized;
        event RectangleToolDraggedEventHandler Dragged;

        int Left { get; set; }
        int Width { get; set; }
        bool AllowResizeWidth { get; set; }
        bool AllowResize { get; set; }
    }
}