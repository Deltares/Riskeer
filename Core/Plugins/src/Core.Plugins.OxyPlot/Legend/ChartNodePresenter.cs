using System.Collections;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Properties;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Forms;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class describes the presentation of <see cref="BaseChart"/> in a <see cref="TreeView"/>.
    /// </summary>
    public class ChartNodePresenter : TreeViewNodePresenterBase<BaseChart>
    {
        public override DragOperations CanDrop(object item, TreeNode sourceNode, TreeNode targetNode, DragOperations validOperations)
        {
            if (item is ChartData)
            {
                return validOperations;
            }
            return base.CanDrop(item, sourceNode, targetNode, validOperations);
        }

        public override bool CanInsert(object item, TreeNode sourceNode, TreeNode targetNode)
        {
            return item is ChartData;
        }

        public override void OnDragDrop(object item, object itemParent, BaseChart target, DragOperations operation, int position)
        {
            var draggedData = item as ChartData;
            target.SetIndex(draggedData, position);
            target.NotifyObservers();
        }

        public override void UpdateNode(TreeNode parentNode, TreeNode node, BaseChart nodeData)
        {
            node.Text = Properties.Resources.General_Chart;
            node.Image = Resources.folder;
        }

        public override IEnumerable GetChildNodeObjects(BaseChart parentNodeData)
        {
            return parentNodeData.Data;
        }
    }
}