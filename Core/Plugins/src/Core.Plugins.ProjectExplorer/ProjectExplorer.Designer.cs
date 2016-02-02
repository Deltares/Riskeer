namespace Core.Plugins.ProjectExplorer
{
    partial class ProjectExplorer
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectExplorer));
            this.treeViewPanel = new System.Windows.Forms.Panel();
            this.projectTreeView = new Core.Common.Controls.TreeView.TreeView();
            this.treeViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewPanel
            // 
            this.treeViewPanel.Controls.Add(this.projectTreeView);
            resources.ApplyResources(this.treeViewPanel, "treeViewPanel");
            this.treeViewPanel.Name = "treeViewPanel";
            // 
            // projectTreeView
            // 
            this.projectTreeView.AllowDrop = true;
            resources.ApplyResources(this.projectTreeView, "projectTreeView");
            this.projectTreeView.HideSelection = false;
            this.projectTreeView.LabelEdit = true;
            this.projectTreeView.Name = "projectTreeView";
            // 
            // ProjectExplorer
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewPanel);
            this.Name = "ProjectExplorer";
            this.treeViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel treeViewPanel;
        private Common.Controls.TreeView.TreeView projectTreeView;
    }
}
