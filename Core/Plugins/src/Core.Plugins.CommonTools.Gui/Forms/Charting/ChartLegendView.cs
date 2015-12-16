using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Swf.Charting;
using Core.Common.Controls.Swf.Charting.Series;
using Core.Common.Controls.Views;
using Core.Common.Gui;

namespace Core.Plugins.CommonTools.Gui.Forms.Charting
{
    public partial class ChartLegendView : UserControl, IView
    {
        private readonly GuiPlugin guiPlugin;
        private readonly IDictionary<ToolStripButton, Action<bool>> actionLookup;
        private IChart chart;

        public ChartLegendView(GuiPlugin guiPlugin)
        {
            this.guiPlugin = guiPlugin;
            InitializeComponent();

            actionLookup = new Dictionary<ToolStripButton, Action<bool>>
            {
                {
                    toolStripButtonStackSeries, b => chart.StackSeries = b
                },
            };

            treeView.SelectedNodeChanged += TreeViewSelectedNodeChanged;
            treeView.RegisterNodePresenter(new ChartTreeNodePresenter());
            treeView.RegisterNodePresenter(new ChartSeriesTreeNodePresenter());
            UpdateButtons();
        }

        public object Data
        {
            get
            {
                return chart;
            }
            set
            {
                chart = (IChart) value;
                treeView.Data = chart;
                UpdateButtons();
            }
        }

        public ViewInfo ViewInfo { get; set; }

        private void TreeViewSelectedNodeChanged(object sender, EventArgs e)
        {
            if (guiPlugin == null || guiPlugin.Gui == null)
            {
                return;
            }

            if (treeView.SelectedNode != null)
            {
                guiPlugin.Gui.Selection = treeView.SelectedNode.Tag;
            }

            UpdateButtons();
        }

        private void UpdateButtons()
        {
            ResetToolBar();

            if (chart == null || !chart.AllowSeriesTypeChange || (treeView.SelectedNode != null && treeView.SelectedNode.Tag is IPolygonChartSeries))
            {
                toolStrip1.Enabled = false;
                return;
            }

            toolStripButtonStackSeries.Checked = chart.StackSeries;
        }

        private void ResetToolBar()
        {
            toolStrip1.Enabled = true;

            foreach (ToolStripButton item in toolStrip1.Items.OfType<ToolStripButton>())
            {
                item.Enabled = true;
                item.Checked = false;
            }
        }

        private void ToolStripButtonClick(object sender, EventArgs e)
        {
            var button = sender as ToolStripButton;
            if (button == null || !actionLookup.ContainsKey(button))
            {
                return;
            }

            button.Checked = !button.Checked;
            actionLookup[button](button.Checked);
        }

        private void ToolStripButtonSeriesClick(object sender, EventArgs e)
        {
            var seriesToChange = GetSeriesToChange();

            foreach (var series in seriesToChange)
            {
                ChangeSeries(sender, series);
            }
        }

        private IEnumerable<ChartSeries> GetSeriesToChange()
        {
            var seriesToChange = new List<ChartSeries>();
            if (treeView.SelectedNode == null)
            {
                return seriesToChange;
            }

            var nodeItem = treeView.SelectedNode.Tag;

            var nodeChart = nodeItem as IChart;
            if (nodeChart != null)
            {
                seriesToChange.AddRange(nodeChart.Series);
            }
            if (nodeItem is ChartSeries)
            {
                seriesToChange.Add(nodeItem as ChartSeries);
            }

            return seriesToChange;
        }

        private void ChangeSeries(object sender, ChartSeries originalSeries)
        {
            if (!chart.AllowSeriesTypeChange)
            {
                return;
            }

            var series = GetNewChartSeries(sender as ToolStripButton, originalSeries);

            var parentNode = treeView.SelectedNode != null ? treeView.SelectedNode.Parent : null;
            var index = chart.GetIndexOfChartSeries(originalSeries);
            if (index < 0)
            {
                chart.AddChartSeries(series);
            }
            else
            {
                chart.InsertChartSeries(series, index);
            }
            chart.RemoveChartSeries(originalSeries);

            if (parentNode != null)
            {
                treeView.SelectedNode = parentNode.GetNodeByTag(series);
            }
        }

        private ChartSeries GetNewChartSeries(ToolStripButton toolStripButton, ChartSeries originalSeries)
        {
            if (originalSeries is IPolygonChartSeries)
            {
                return originalSeries;
            }

            if (toolStripButton == toolStripButtonAreaSeries)
            {
                return (ChartSeries)ChartSeriesFactory.CreateAreaSeries(originalSeries);
            }

            if (toolStripButton == toolStripButtonBarSeries)
            {
                return (ChartSeries)ChartSeriesFactory.CreateBarSeries(originalSeries);
            }

            if (toolStripButton == toolStripButtonLineSeries)
            {
                return (ChartSeries)ChartSeriesFactory.CreateLineSeries(originalSeries);
            }

            if (toolStripButton == toolStripButtonPointSeries)
            {
                return (ChartSeries)ChartSeriesFactory.CreatePointSeries(originalSeries);
            }

            throw new NotImplementedException();
        }
    }
}