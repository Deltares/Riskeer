using System;
using Core.Common.Controls.TreeView;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Properties;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class describes the presentation of <see cref="ChartData"/> in a <see cref="TreeView"/>.
    /// </summary>
    public class ChartDataNodePresenter : TreeViewNodePresenterBase<ChartData>
    {
        public override DragOperations CanDrag(ChartData nodeData)
        {
            return DragOperations.Move;
        }

        public override void UpdateNode(TreeNode parentNode, TreeNode node, ChartData nodeData)
        {
            if (nodeData is AreaData)
            {
                node.Text = Resources.ChartDataNodePresenter_Area_data_label;
                node.Image = Resources.AreaIcon;
                var isVisible = ((AreaData)nodeData).IsVisible;
                if (node.Checked != isVisible)
                {
                    node.Checked = isVisible;
                }
            }
            else if (nodeData is LineData)
            {
                node.Text = Resources.ChartDataNodePresenter_Line_data_label;
                node.Image = Resources.LineIcon;
                var isVisible = ((LineData)nodeData).IsVisible;
                if (node.Checked != isVisible)
                {
                    node.Checked = isVisible;
                }
            }
            else if (nodeData is PointData)
            {
                node.Text = Resources.ChartDataNodePresenter_Point_data_label;
                node.Image = Resources.PointsIcon;
                var isVisible = ((PointData)nodeData).IsVisible;
                if (node.Checked != isVisible)
                {
                    node.Checked = isVisible;
                }
            }
            else
            {
                throw new NotSupportedException("Cannot add chart data of type other than points, lines or area.");
            }
            node.ShowCheckBox = true;
        }

        public override void OnNodeChecked(TreeNode node)
        {
            var chartData = (ChartData)node.Parent.Tag;
            var lineData = node.Tag as LineData;
            var pointData = node.Tag as PointData;
            var areaData = node.Tag as AreaData;
            if (lineData != null)
            {
                lineData.IsVisible = node.Checked;
            }
            if (pointData != null)
            {
                pointData.IsVisible = node.Checked;
            }
            if (areaData != null)
            {
                areaData.IsVisible = node.Checked;
            }
            chartData.NotifyObservers();
        }
    }
}