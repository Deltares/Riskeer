namespace DelftTools.Controls.Swf.Charting
{
    public interface IChartViewZoom
    {
        bool Animated { get; set; }
        bool Allow { get; set; }
        ZoomDirections Direction { get; set; }
    }
}