using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Properties;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class defines a view which shows the data that have been added to a <see cref="BaseChart"/>.
    /// </summary>
    public sealed partial class LegendView : UserControl, IView
    {
        /// <summary>
        /// Creates a new instance of <see cref="LegendView"/>.
        /// </summary>
        public LegendView()
        {
            InitializeComponent();
            Text = Resources.General_Chart;
        }

        public object Data
        {
            get
            {
                return seriesTree.Chart;
            }
            set
            {
                UpdateTree((BaseChart)value);
            }
        }

        /// <summary>
        /// Updates the tree with the current state of the chart.
        /// </summary>
        private void UpdateTree(BaseChart data)
        {
            if (IsDisposed)
            {
                return;
            }
            seriesTree.Chart = data;
        }
    }
}