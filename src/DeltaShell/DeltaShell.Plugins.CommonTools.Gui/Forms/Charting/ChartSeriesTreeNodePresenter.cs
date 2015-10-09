using System.Drawing;
using DelftTools.Controls;
using DelftTools.Controls.Swf.Charting;
using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Swf;
using DeltaShell.Plugins.CommonTools.Gui.Properties;

namespace DeltaShell.Plugins.CommonTools.Gui.Forms.Charting
{
    internal class ChartSeriesTreeNodePresenter : TreeViewNodePresenterBaseForPluginGui<IChartSeries>
    {
        public ChartSeriesTreeNodePresenter(GuiPlugin guiPlugin) : base(guiPlugin) {}

        public override bool CanRenameNode(ITreeNode node)
        {
            return true;
        }

        public override void OnNodeRenamed(IChartSeries chartSeries, string newName)
        {
            if (chartSeries == null || chartSeries.Title == newName)
            {
                return;
            }

            chartSeries.Title = newName;
        }

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, IChartSeries chartSeries)
        {
            node.Text = chartSeries.Title;
            node.Tag = chartSeries;
            node.Checked = chartSeries.Visible;
            node.ShowCheckBox = true;
            node.ContextMenu = GetContextMenu(null, chartSeries);
            node.Image = GetImage(chartSeries);
        }

        public override void OnNodeChecked(ITreeNode node)
        {
            var chartSeries = (IChartSeries) node.Tag;

            if (chartSeries != null)
            {
                chartSeries.Visible = node.Checked;
            }
        }

        public override DragOperations CanDrag(IChartSeries nodeData)
        {
            return DragOperations.Move;
        }

        protected override bool RemoveNodeData(object parentNodeData, IChartSeries chartSeries)
        {
            chartSeries.Chart.Series.Remove(chartSeries);
            return true;
        }

        private Image GetImage(IChartSeries chartSeries)
        {
            if (chartSeries is IAreaChartSeries)
            {
                return Resources.Area;
            }

            if (chartSeries is BarSeries)
            {
                return Resources.Bars;
            }

            if (chartSeries is ILineChartSeries)
            {
                return Resources.Line;
            }

            if (chartSeries is IPointChartSeries)
            {
                return Resources.Points;
            }

            if (chartSeries is IPolygonChartSeries)
            {
                return Resources.Polygon;
            }

            return null;
        }
    }
}