namespace Core.Common.Controls.Charting
{
    public interface IChartViewSeriesTool : IChartViewTool
    {
        IChartSeries Series { get; }
    }
}