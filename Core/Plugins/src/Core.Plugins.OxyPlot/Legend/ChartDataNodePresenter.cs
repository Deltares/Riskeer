using Core.Common.Controls.TreeView;
using Core.Components.OxyPlot.Data;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class describes the presentation of <see cref="IChartData"/> in a <see cref="TreeView"/>.
    /// </summary>
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