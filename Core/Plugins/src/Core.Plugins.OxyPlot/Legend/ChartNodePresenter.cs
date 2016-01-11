using System.Collections;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Properties;
using Core.Components.OxyPlot.Forms;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class describes the presentation of <see cref="BaseChart"/> in a <see cref="TreeView"/>.
    /// </summary>
    public class ChartNodePresenter : TreeViewNodePresenterBase<BaseChart>
    {
        public override void UpdateNode(TreeNode parentNode, TreeNode node, BaseChart nodeData)
        {
            node.Text = Properties.Resources.General_Chart;
            node.Image = Resources.folder;
        }

        public override IEnumerable GetChildNodeObjects(BaseChart parentNodeData)
        {
            return parentNodeData.Model.Series;
        }
    }
}