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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectExplorer));
            this.treeViewPanel = new System.Windows.Forms.Panel();
            this.treeViewControl = new Core.Common.Controls.TreeView.TreeViewControl();
            this.treeViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewPanel
            // 
            this.treeViewPanel.Controls.Add(this.treeViewControl);
            resources.ApplyResources(this.treeViewPanel, "treeViewPanel");
            this.treeViewPanel.Name = "treeViewPanel";
            // 
            // treeViewControl
            // 
            resources.ApplyResources(this.treeViewControl, "treeViewControl");
            this.treeViewControl.Name = "treeViewControl";
            // 
            // ProjectExplorer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewPanel);
            this.Name = "ProjectExplorer";
            this.treeViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel treeViewPanel;
        private Common.Controls.TreeView.TreeViewControl treeViewControl;
    }
}
