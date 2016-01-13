using OxyPlot.Series;

namespace Core.Components.OxyPlot.Data
{
    internal interface ISeries : IChartData {
        Series Series
        {
            get;
        }
    }
}