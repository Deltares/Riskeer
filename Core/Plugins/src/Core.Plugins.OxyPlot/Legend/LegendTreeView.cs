using System.Collections;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Properties;
using Core.Components.OxyPlot;
using Core.Components.OxyPlot.Data;

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
            node.Image = Resources.folder;
        }

        public override IEnumerable GetChildNodeObjects(BaseChart parentNodeData)
        {
            return parentNodeData.Model.Series;
        }
    }

    public class ChartDataNodePresenter : TreeViewNodePresenterBase<IChartData>
    {
        public override void UpdateNode(TreeNode parentNode, TreeNode node, IChartData nodeData)
        {
            node.Text = nodeData.GetType().Name;
            if (nodeData is AreaData)
            {
                node.Image = Properties.Resources.AreaIcon;
            }
            if (nodeData is LineData)
            {
                node.Image = Properties.Resources.LineIcon;
            }
            if (nodeData is PointData)
            {
                node.Image = Properties.Resources.PointsIcon;
            }
        }
    }
}