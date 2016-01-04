using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.OxyPlot;

namespace Core.Plugins.OxyPlot.Forms
{
    public class ChartDataView : UserControl, IView
    {
        private readonly BaseChart baseChart;
        private ChartData data;

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
                data = (ChartData) value;
                if (data != null)
                {
                    baseChart.AddData(data);
                }
            }
        }
    }
}