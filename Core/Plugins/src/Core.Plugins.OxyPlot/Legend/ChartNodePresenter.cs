using System.Collections;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Properties;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Collection;
using Core.Components.OxyPlot.Forms;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class describes the presentation of <see cref="BaseChart"/> in a <see cref="TreeView"/>.
    /// </summary>
    public class ChartNodePresenter : TreeViewNodePresenterBase<ChartDataCollection>
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

        public override void OnDragDrop(object item, object itemParent, ChartDataCollection target, DragOperations operation, int position)
        {
            var draggedData = item as ChartData;
            target.List.Remove(draggedData);
            target.List.Insert(target.List.Count - position, draggedData);
            target.NotifyObservers();
        }

        public override void UpdateNode(TreeNode parentNode, TreeNode node, ChartDataCollection nodeData)
        {
            node.Text = Properties.Resources.General_Chart;
            node.Image = Resources.folder;
        }

        public override IEnumerable GetChildNodeObjects(ChartDataCollection parentNodeData)
        {
            return parentNodeData.List.Reverse();
        }
    }
}