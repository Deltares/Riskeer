using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.OxyPlot;
using Core.Plugins.OxyPlot.Properties;
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Plugins.OxyPlot.Legend
{
    public sealed partial class LegendView : UserControl, IView
    {
        private BaseChart chart;
        private TreeView treeView;

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
            }
        }
    }
}