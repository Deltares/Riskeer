namespace DelftTools.Controls.Swf.Charting.Tools
{
    public interface IHistoryTool : IChartViewTool
    {
        bool ShowToolTip { get; set; }
        void Add(IChartSeries series);

        /// <summary>
        /// Remove all history series
        /// </summary>
        void ClearHistory();
    }
}