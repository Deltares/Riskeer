using Core.Common.Controls.TreeView;

namespace Core.Plugins.CommonTools.Gui.Forms.Charting
{
    partial class ChartLegendView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartLegendView));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonStackSeries = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAreaSeries = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLineSeries = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPointSeries = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonBarSeries = new System.Windows.Forms.ToolStripButton();
            this.treeView = new TreeView();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonStackSeries,
            this.toolStripSeparator1,
            this.toolStripButtonAreaSeries,
            this.toolStripButtonLineSeries,
            this.toolStripButtonPointSeries,
            this.toolStripButtonBarSeries});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripButtonStackSeries
            // 
            this.toolStripButtonStackSeries.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStackSeries.Image = global::Core.Plugins.CommonTools.Gui.Properties.Resources.Stacked;
            resources.ApplyResources(this.toolStripButtonStackSeries, "toolStripButtonStackSeries");
            this.toolStripButtonStackSeries.Name = "toolStripButtonStackSeries";
            this.toolStripButtonStackSeries.Click += new System.EventHandler(this.ToolStripButtonClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonAreaSeries
            // 
            this.toolStripButtonAreaSeries.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAreaSeries.Image = global::Core.Plugins.CommonTools.Gui.Properties.Resources.Area;
            resources.ApplyResources(this.toolStripButtonAreaSeries, "toolStripButtonAreaSeries");
            this.toolStripButtonAreaSeries.Name = "toolStripButtonAreaSeries";
            this.toolStripButtonAreaSeries.Click += new System.EventHandler(this.ToolStripButtonSeriesClick);
            // 
            // toolStripButtonLineSeries
            // 
            this.toolStripButtonLineSeries.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLineSeries.Image = global::Core.Plugins.CommonTools.Gui.Properties.Resources.Line;
            resources.ApplyResources(this.toolStripButtonLineSeries, "toolStripButtonLineSeries");
            this.toolStripButtonLineSeries.Name = "toolStripButtonLineSeries";
            this.toolStripButtonLineSeries.Click += new System.EventHandler(this.ToolStripButtonSeriesClick);
            // 
            // toolStripButtonPointSeries
            // 
            this.toolStripButtonPointSeries.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPointSeries.Image = global::Core.Plugins.CommonTools.Gui.Properties.Resources.Points;
            resources.ApplyResources(this.toolStripButtonPointSeries, "toolStripButtonPointSeries");
            this.toolStripButtonPointSeries.Name = "toolStripButtonPointSeries";
            this.toolStripButtonPointSeries.Click += new System.EventHandler(this.ToolStripButtonSeriesClick);
            // 
            // toolStripButtonBarSeries
            // 
            this.toolStripButtonBarSeries.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBarSeries.Image = global::Core.Plugins.CommonTools.Gui.Properties.Resources.Bars;
            resources.ApplyResources(this.toolStripButtonBarSeries, "toolStripButtonBarSeries");
            this.toolStripButtonBarSeries.Name = "toolStripButtonBarSeries";
            this.toolStripButtonBarSeries.Click += new System.EventHandler(this.ToolStripButtonSeriesClick);
            // 
            // treeView
            // 
            this.treeView.AllowDrop = true;
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.treeView.HideSelection = false;
            this.treeView.LabelEdit = true;
            this.treeView.Name = "treeView";
            this.treeView.SelectNodeOnRightMouseClick = true;
            // 
            // ChartLegendView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ChartLegendView";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TreeView treeView;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonStackSeries;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAreaSeries;
        private System.Windows.Forms.ToolStripButton toolStripButtonLineSeries;
        private System.Windows.Forms.ToolStripButton toolStripButtonPointSeries;
        private System.Windows.Forms.ToolStripButton toolStripButtonBarSeries;
    }
}