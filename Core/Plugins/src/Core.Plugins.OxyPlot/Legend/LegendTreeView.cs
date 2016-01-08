using System.Collections;
using Core.Common.Controls.TreeView;
using Core.Components.OxyPlot;
using OxyPlot.Series;

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

    public class ChartNodePresenter : TreeViewNodePresenterBase<BaseChart>
    {
        public override void UpdateNode(TreeNode parentNode, TreeNode node, BaseChart nodeData)
        {
            node.Text = "Grafiek";
        }

        public override IEnumerable GetChildNodeObjects(BaseChart parentNodeData)
        {
            return parentNodeData.Model.Series;
        }
    }

    public class ChartDataNodePresenter : TreeViewNodePresenterBase<ItemsSeries>
    {
        public override void UpdateNode(TreeNode parentNode, TreeNode node, ItemsSeries nodeData)
        {
            node.Text = nodeData.GetType().Name;
        }
    }
}