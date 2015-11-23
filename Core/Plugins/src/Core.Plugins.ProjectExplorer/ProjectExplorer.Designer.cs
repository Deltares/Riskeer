namespace Core.Plugins.ProjectExplorer
{
    partial class ProjectExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectExplorer));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonScrollToItemInActiveView = new System.Windows.Forms.ToolStripButton();
            this.treeViewPanel = new System.Windows.Forms.Panel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonScrollToItemInActiveView});
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // buttonScrollToItemInActiveView
            // 
            resources.ApplyResources(this.buttonScrollToItemInActiveView, "buttonScrollToItemInActiveView");
            this.buttonScrollToItemInActiveView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonScrollToItemInActiveView.Name = "buttonScrollToItemInActiveView";
            this.buttonScrollToItemInActiveView.Tag = "LocateActiveViewItem";
            this.buttonScrollToItemInActiveView.Click += new System.EventHandler(this.ButtonScrollToItemInActiveViewClick);
            // 
            // treeViewPanel
            // 
            resources.ApplyResources(this.treeViewPanel, "treeViewPanel");
            this.treeViewPanel.Name = "treeViewPanel";
            // 
            // ProjectExplorer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewPanel);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ProjectExplorer";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonScrollToItemInActiveView;
        private System.Windows.Forms.Panel treeViewPanel;
    }
}
