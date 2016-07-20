using System.Windows.Forms;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;

namespace Demo.Ringtoets.Views
{
    /// <summary>
    /// This class represents a simple view with a chart, to which data can be added.
    /// </summary>
    public partial class ChartDataView : UserControl, IChartView
    {
        private ChartDataCollection data;

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
                data = (ChartDataCollection)value;

                if (data != null)
                {
                    foreach (var chartData in data.List)
                    {
                        Chart.Data.Add(chartData);
                    }

                    Chart.Data.NotifyObservers();
                }
            }
        }

        public IChartControl Chart
        {
            get
            {
                return chartControl;
            }
        }
    }
}