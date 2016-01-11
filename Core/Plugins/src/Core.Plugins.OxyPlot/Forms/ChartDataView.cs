using System.Windows.Forms;
using Core.Components.OxyPlot.Data;
using Core.Components.OxyPlot.Forms;

namespace Core.Plugins.OxyPlot.Forms
{
    /// <summary>
    /// This class represents a simple view with a chart, to which data can be added.
    /// </summary>
    public class ChartDataView : UserControl, IChartView
    {
        private readonly BaseChart baseChart;
        private IChartData data;

        /// <summary>
        /// Creates an instance of <see cref="ChartDataView"/> with just a <see cref="BaseChart"/> on it.
        /// </summary>
        public ChartDataView()
        {
            baseChart = new BaseChart
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(baseChart);
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                baseChart.ClearData();
                data = (IChartData) value;
                if (data != null)
                {
                    baseChart.AddData(data);
                }
            }
        }

        public BaseChart Chart
        {
            get
            {
                return baseChart;
            }
        }
    }
}