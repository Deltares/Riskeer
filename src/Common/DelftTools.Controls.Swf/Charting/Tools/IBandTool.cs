namespace DelftTools.Controls.Swf.Charting.Tools
{

    public interface ISeriesBandTool : IChartViewTool
    {
        IChartSeries Series { get; set; }
        IChartSeries Series2 { get; set; }

    }

    public interface IBandTool : IChartViewTool
    {
        
    }

}