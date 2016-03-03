using System.Windows.Forms;
using Core.Components.Charting;
using Core.Components.Charting.Forms;
using Core.Components.OxyPlot.Forms;

namespace Core.Plugins.OxyPlot.Test
{
    /// <summary>
    /// Simple <see cref="IChartView"/> implementation which can be used in tests.
    /// </summary>
    public class TestChartView : Control, IChartView
    {
        public object Data { get; set; }

        public IChart Chart
        {
            get
            {
                return (BaseChart) Data;
            }
        }
    }
}