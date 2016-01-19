using System.Collections.Generic;
using System.Windows.Forms;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Forms;

namespace Core.Plugins.OxyPlot.Forms
{
    /// <summary>
    /// This class represents a simple view with a chart, to which data can be added.
    /// </summary>
    public partial class ChartDataView : UserControl, IChartView
    {
        private ICollection<ChartData> data;

        /// <summary>
        /// Creates a new instance of <see cref="ChartDataView"/>.
        /// </summary>
        public ChartDataView()
        {
            InitializeComponent();
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
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        Chart.Data.Add(item);
                    }
                }
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