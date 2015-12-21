using System.Drawing;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Common.Controls.TreeView;
using Core.Plugins.Charting.Properties;

namespace Core.Plugins.Charting.Forms
{
    public class ChartSeriesTreeNodePresenter : TreeViewNodePresenterBase<ChartSeries>
    {
        public override bool CanRenameNode(ITreeNode node)
        {
            return true;
        }

        public override void OnNodeRenamed(ChartSeries chartSeries, string newName)
        {
            if (chartSeries == null || chartSeries.Title == newName)
            {
                return;
            }

            chartSeries.Title = newName;
        }

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, ChartSeries chartSeries)
        {
            node.Text = chartSeries.Title;
            node.Tag = chartSeries;
            node.Checked = chartSeries.Visible;
            node.ShowCheckBox = true;
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

        public override DragOperations CanDrag(ChartSeries nodeData)
        {
            return DragOperations.Move;
        }

        protected override bool RemoveNodeData(object parentNodeData, ChartSeries chartSeries)
        {
            return chartSeries.Chart.RemoveChartSeries(chartSeries);
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