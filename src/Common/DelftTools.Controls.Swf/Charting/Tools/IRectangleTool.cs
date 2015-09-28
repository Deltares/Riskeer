namespace DelftTools.Controls.Swf.Charting.Tools
{
    public interface IRectangleTool : IChartViewTool
    {

        int Left { get; set;  }
        int Width { get; set; }
        bool AllowResizeWidth { get; set; }
        bool AllowResize { get; set; }
        event RectangleToolDraggingEventHandler Dragging;
        event RectangleToolResizingEventHandler Resizing;
        event RectangleToolResizedEventHandler Resized;
        event RectangleToolDraggedEventHandler Dragged;
        
    }
}