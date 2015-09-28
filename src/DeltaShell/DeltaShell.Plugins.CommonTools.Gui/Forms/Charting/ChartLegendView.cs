using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Controls.Swf.Charting;
using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.Shell.Gui;

namespace DeltaShell.Plugins.CommonTools.Gui.Forms.Charting
{
    public partial class ChartLegendView : UserControl, IView
    {
        private readonly GuiPlugin guiPlugin;
        private IChart chart;
        private readonly IDictionary<ToolStripButton, Action<bool>> actionLookup;  

        public ChartLegendView(GuiPlugin guiPlugin)
        {
            this.guiPlugin = guiPlugin;
            InitializeComponent();

            actionLookup = new Dictionary<ToolStripButton, Action<bool>>
                               {
                                   {toolStripButtonStackSeries, b => chart.StackSeries = b},
                                   
                               };

            treeView.SelectedNodeChanged += TreeViewSelectedNodeChanged;
            treeView.NodePresenters.Add(new ChartTreeNodePresenter(guiPlugin));
            treeView.NodePresenters.Add(new ChartSeriesTreeNodePresenter(guiPlugin));
            UpdateButtons();
        }

        public object Data
        {
            get { return chart; }
            set
            {
                chart = (IChart) value;
                treeView.Data = chart;
                UpdateButtons();
            }
        }

        public Image Image { get; set; }

        public void EnsureVisible(object item) { }
        public ViewInfo ViewInfo { get; set; }

        private void TreeViewSelectedNodeChanged(object sender, EventArgs e)
        {
            if (guiPlugin == null || guiPlugin.Gui == null) return;

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
            if (button == null || !actionLookup.ContainsKey(button)) return;
            
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

        private IEnumerable<IChartSeries> GetSeriesToChange()
        {
            var seriesToChange = new List<IChartSeries>();
            if (treeView.SelectedNode == null) return seriesToChange;

            var nodeItem = treeView.SelectedNode.Tag;
            
            if (nodeItem is IChart)
            {
                seriesToChange.AddRange(((IChart)nodeItem).Series);
            }
            if (nodeItem is ChartSeries)
            {
                seriesToChange.Add(nodeItem as ChartSeries);
            }

            return seriesToChange;
        }

        private void ChangeSeries(object sender, IChartSeries originalSeries)
        {
            if (!chart.AllowSeriesTypeChange)
            {
                return;
            }

            
            var series = GetNewChartSeries(sender as ToolStripButton, originalSeries);

            var parentNode = treeView.SelectedNode != null ? treeView.SelectedNode.Parent : null;
            var index = chart.Series.IndexOf(originalSeries);
            if (index < 0)
                chart.Series.Add(series);
            else 
                chart.Series.Insert(index, series);
            chart.Series.Remove(originalSeries);

            if (parentNode != null)
            {
                treeView.SelectedNode = parentNode.GetNodeByTag(series);
            }
        }

        private IChartSeries GetNewChartSeries(ToolStripButton toolStripButton, IChartSeries originalSeries)
        {
            if (originalSeries is IPolygonChartSeries)
            {
                return originalSeries;
            } 
            
            if (toolStripButton == toolStripButtonAreaSeries)
            {
                return ChartSeriesFactory.CreateAreaSeries(originalSeries);
            }

            if (toolStripButton == toolStripButtonBarSeries)
            {
                return ChartSeriesFactory.CreateBarSeries(originalSeries);
            }

            if (toolStripButton == toolStripButtonLineSeries)
            {
                return ChartSeriesFactory.CreateLineSeries(originalSeries);
            }

            if (toolStripButton == toolStripButtonPointSeries)
            {
                return ChartSeriesFactory.CreatePointSeries(originalSeries);
            }
            
            throw new NotImplementedException();
        }
    }
}