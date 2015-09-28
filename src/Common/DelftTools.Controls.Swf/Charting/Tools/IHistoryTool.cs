namespace DelftTools.Controls.Swf.Charting.Tools
{
    public interface IHistoryTool : IChartViewTool
    {
        void Add(IChartSeries series);

        /// <summary>
        /// Remove all history series
        /// </summary>
        void ClearHistory();

        bool ShowToolTip { get; set; }
    }
}