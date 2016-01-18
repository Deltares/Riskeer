using System.Collections.Generic;
using System.Windows.Forms;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Forms;
using UserControl = System.Windows.Forms.UserControl;

namespace Core.Plugins.OxyPlot.Forms
{
    /// <summary>
    /// This class represents a simple view with a chart, to which data can be added.
    /// </summary>
    public class ChartDataView : UserControl, IChartView
    {
        private readonly BaseChart chart;
        private ICollection<ChartData> data;

        /// <summary>
        /// Creates an instance of <see cref="ChartDataView"/> with just a <see cref="BaseChart"/> on it.
        /// </summary>
        public ChartDataView()
        {
            chart = new BaseChart
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(chart);
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = (ICollection<ChartData>) value;
                chart.Data = data;
            }
        }

        public BaseChart Chart
        {
            get
            {
                return chart;
            }
        }
    }
}