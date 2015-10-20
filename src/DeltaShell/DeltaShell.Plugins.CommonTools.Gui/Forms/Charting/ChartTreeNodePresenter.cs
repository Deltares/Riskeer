using System.Collections;
using System.Linq;
using DelftTools.Controls;
using DelftTools.Controls.Swf.Charting;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Swf;
using DeltaShell.Plugins.CommonTools.Gui.Properties;

namespace DeltaShell.Plugins.CommonTools.Gui.Forms.Charting
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
            node.ContextMenuStrip = GetContextMenu(null, chart);
        }

        public override IEnumerable GetChildNodeObjects(IChart chart, ITreeNode node)
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