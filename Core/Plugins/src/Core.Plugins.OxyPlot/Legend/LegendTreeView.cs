using Core.Components.OxyPlot.Forms;
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Plugins.OxyPlot.Legend
{
    public class LegendTreeView : TreeView
    {
        public LegendTreeView()
        {
            RegisterNodePresenter(new ChartDataNodePresenter());
            RegisterNodePresenter(new ChartNodePresenter());
        }

        public BaseChart Chart
        {
            get
            {
                return (BaseChart)Data;
            }
            set
            {
                Data = value;
                
                if (value == null)
                {
                    Nodes.Clear();
                }
            }
        }
    }
}