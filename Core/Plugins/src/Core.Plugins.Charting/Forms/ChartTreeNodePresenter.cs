using System.Collections;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Common.Controls.TreeView;
using Core.Plugins.Charting.Properties;

namespace Core.Plugins.Charting.Forms
{
    public class ChartTreeNodePresenter : TreeViewNodePresenterBase<IChart>
    {
        public override bool CanRenameNode(TreeNode node)
        {
            return true;
        }

        public override void UpdateNode(TreeNode parentNode, TreeNode node, IChart chart)
        {
            node.Tag = chart;
            node.Text = string.IsNullOrEmpty(chart.Title) ? Resources.ChartingGuiPlugin_Chart : chart.Title;
            node.Image = Resources.Chart;
        }

        public override void OnNodeRenamed(IChart chart, string newName)
        {
            if (chart == null || chart.Title == newName)
            {
                return;
            }

            chart.Title = newName;
        }

        public override IEnumerable GetChildNodeObjects(IChart chart)
        {
            return chart.Series;
        }

        public override bool CanInsert(object item, TreeNode sourceNode, TreeNode targetNode)
        {
            return (null == TreeView.TreeViewNodeSorter);
        }

        public override DragOperations CanDrop(object item, TreeNode sourceNode, TreeNode targetNode, DragOperations validOperations)
        {
            if (item is IChartSeries)
            {
                return GetDefaultDropOperation(validOperations);
            }

            return base.CanDrop(item, sourceNode, targetNode, validOperations);
        }

        public override void OnDragDrop(object item, object itemParent, IChart target, DragOperations operation, int position)
        {
            var series = item as ChartSeries;

            if (series == null)
            {
                return;
            }

            var chart = itemParent as IChart;
            if (chart != null)
            {
                chart.RemoveChartSeries(series);
            }
            else
            {
                target.RemoveChartSeries(series);
            }

            target.InsertChartSeries(series, position);
        }
    }
}