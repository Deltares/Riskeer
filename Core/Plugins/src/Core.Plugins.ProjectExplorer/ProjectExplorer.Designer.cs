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
            this.treeViewPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
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
            this.Name = "ProjectExplorer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel treeViewPanel;
    }
}
