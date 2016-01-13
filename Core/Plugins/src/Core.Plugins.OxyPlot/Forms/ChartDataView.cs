using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Forms;
using Core.Components.OxyPlot.Data;
using Core.Components.OxyPlot.Forms;
using UserControl = System.Windows.Forms.UserControl;

namespace Core.Plugins.OxyPlot.Forms
{
    /// <summary>
    /// This class represents a simple view with a chart, to which data can be added.
    /// </summary>
    public class ChartDataView : UserControl, IChartView
    {
        private readonly BaseChart baseChart;
        private ICollection<IChartData> data;

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
                data = (ICollection<IChartData>) value;
                if (data != null)
                {
                    foreach (var series in data)
                    {
                        baseChart.AddData(series);
                    }
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