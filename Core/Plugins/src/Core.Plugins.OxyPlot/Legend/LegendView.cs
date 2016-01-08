using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.OxyPlot;
using Core.Plugins.OxyPlot.Properties;

namespace Core.Plugins.OxyPlot.Legend
{
    public sealed partial class LegendView : UserControl, IView
    {
        private BaseChart chart;

        public LegendView()
        {
            InitializeComponent();
            Text = Resources.Ribbon_Chart;
        }

        public object Data
        {
            get
            {
                return chart;
            }
            set
            {
                chart = (BaseChart) value;
                UpdateTree();
            }
        }

        private void UpdateTree()
        {
            if (IsDisposed)
            {
                return;
            }
            if (chart != null)
            {
                seriesTree.Data = chart;
            }
            else
            {
                seriesTree.Nodes.Clear();
            }
        }
    }
}