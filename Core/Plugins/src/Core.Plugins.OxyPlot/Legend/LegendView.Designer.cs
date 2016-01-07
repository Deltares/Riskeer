using Core.Common.Controls.TreeView;

namespace Core.Plugins.OxyPlot.Legend
{
    partial class LegendView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LegendView));
            this.toolBar = new System.Windows.Forms.ToolStrip();
            this.treeView = new Core.Common.Controls.TreeView.TreeView();
            this.seriesTree = new Core.Common.Controls.TreeView.TreeView();
            this.SuspendLayout();
            // 
            // toolBar
            // 
            this.toolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            resources.ApplyResources(this.toolBar, "toolBar");
            this.toolBar.Name = "toolBar";
            // 
            // treeView
            // 
            this.treeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.treeView.HideSelection = false;
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.LabelEdit = true;
            this.treeView.LineColor = System.Drawing.Color.Empty;
            this.treeView.Name = "treeView";
            this.treeView.SelectNodeOnRightMouseClick = true;
            // 
            // seriesTree
            // 
            this.seriesTree.AllowDrop = true;
            resources.ApplyResources(this.seriesTree, "seriesTree");
            this.seriesTree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.seriesTree.HideSelection = false;
            this.seriesTree.LabelEdit = true;
            this.seriesTree.Name = "seriesTree";
            this.seriesTree.SelectNodeOnRightMouseClick = true;
            // 
            // LegendView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.seriesTree);
            this.Controls.Add(this.toolBar);
            this.Name = "LegendView";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolBar;
        private TreeView seriesTree;
    }
}