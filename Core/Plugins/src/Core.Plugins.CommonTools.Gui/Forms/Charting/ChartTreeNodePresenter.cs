using System.Collections;
using System.Linq;
using Core.Common.Controls;
using Core.Common.Controls.Swf.Charting;
using Core.Common.Gui;
using Core.Common.Gui.Swf;
using Core.Plugins.CommonTools.Gui.Properties;

namespace Core.Plugins.CommonTools.Gui.Forms.Charting
{
    public class ChartTreeNodePresenter : TreeViewNodePresenterBaseForPluginGui<IChart>
    {
        public ChartTreeNodePresenter(GuiPlugin guiPlugin) : base(guiPlugin) {}

        public override bool CanRenameNode(ITreeNode node)
        {
            return true;
        }

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, IChart chart)
        {
            node.Tag = chart;
            node.Text = string.IsNullOrEmpty(chart.Title) ? Resources.ChartTreeNodePresenter_UpdateNode_Chart : chart.Title;
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
            return chart.Series.Cast<object>();
        }

        public override DragOperations CanDrop(object item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations)
        {
            if (item is IChartSeries)
            {
                return GetDefaultDropOperation(TreeView, item, sourceNode, targetNode, validOperations);
            }

            return base.CanDrop(item, sourceNode, targetNode, validOperations);
        }

        public override void OnDragDrop(object item, object sourceParentNodeData, IChart target, DragOperations operation, int position)
        {
            var series = item as IChartSeries;

            if (series == null)
            {
                return;
            }

            var chart = sourceParentNodeData as IChart;
            if (chart != null)
            {
                chart.Series.Remove(series);
            }
            else
            {
                target.Series.Remove(series);
            }

            target.Series.Insert(position, series);
        }
    }
}