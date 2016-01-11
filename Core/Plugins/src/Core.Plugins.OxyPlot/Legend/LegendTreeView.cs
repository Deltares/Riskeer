using Core.Common.Controls.TreeView;

namespace Core.Plugins.OxyPlot.Legend
{
    public class LegendTreeView : TreeView
    {
        public LegendTreeView()
        {
            RegisterNodePresenter(new ChartDataNodePresenter());
            RegisterNodePresenter(new ChartNodePresenter());
        }
    }
}