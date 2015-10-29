using Core.Common.Controls.Swf;

namespace Core.Plugins.ProjectExplorer
{
    partial class ProjectTreeView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectTreeView));
            this.contextMenuProject = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.buttonFolderAdd = new ClonableToolStripMenuItem();
            this.buttonFolderAddNewItem = new ClonableToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonFolderImportFolder = new ClonableToolStripMenuItem();
            this.buttonFolderExportFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonFolderDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonFolderRename = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonFolderExpandAll = new ClonableToolStripMenuItem();
            this.buttonFolderCollapseAll = new ClonableToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonFolderProperties = new ClonableToolStripMenuItem();
            this.contextMenuProject.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuProject
            // 
            this.contextMenuProject.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonFolderAdd,
            this.toolStripSeparator4,
            this.buttonFolderImportFolder,
            this.buttonFolderExportFolder,
            this.toolStripSeparator18,
            this.buttonFolderDelete,
            this.buttonFolderRename,
            this.toolStripSeparator5,
            this.buttonFolderExpandAll,
            this.buttonFolderCollapseAll,
            this.toolStripSeparator7,
            this.buttonFolderProperties});
            this.contextMenuProject.Name = "contextMenuProject";
            resources.ApplyResources(this.contextMenuProject, "contextMenuProject");
            // 
            // buttonFolderAdd
            // 
            this.buttonFolderAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonFolderAddNewItem});
            this.buttonFolderAdd.Image = global::Core.Plugins.ProjectExplorer.Properties.Resources.plus;
            this.buttonFolderAdd.Name = "buttonFolderAdd";
            resources.ApplyResources(this.buttonFolderAdd, "buttonFolderAdd");
            // 
            // buttonFolderAddNewItem
            // 
            this.buttonFolderAddNewItem.Image = global::Core.Plugins.ProjectExplorer.Properties.Resources.DataItem;
            this.buttonFolderAddNewItem.Name = "buttonFolderAddNewItem";
            resources.ApplyResources(this.buttonFolderAddNewItem, "buttonFolderAddNewItem");
            this.buttonFolderAddNewItem.Click += new System.EventHandler(this.buttonsAddNewDataClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // buttonFolderImportFolder
            // 
            this.buttonFolderImportFolder.Image = global::Core.Plugins.ProjectExplorer.Properties.Resources.table_import;
            this.buttonFolderImportFolder.Name = "buttonFolderImportFolder";
            resources.ApplyResources(this.buttonFolderImportFolder, "buttonFolderImportFolder");
            this.buttonFolderImportFolder.Click += new System.EventHandler(this.buttonFolderImportClick);
            // 
            // buttonFolderExportFolder
            // 
            this.buttonFolderExportFolder.Image = global::Core.Plugins.ProjectExplorer.Properties.Resources.table_export;
            this.buttonFolderExportFolder.Name = "buttonFolderExportFolder";
            resources.ApplyResources(this.buttonFolderExportFolder, "buttonFolderExportFolder");
            this.buttonFolderExportFolder.Click += new System.EventHandler(this.ButtonExportClick);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            resources.ApplyResources(this.toolStripSeparator18, "toolStripSeparator18");
            // 
            // buttonFolderDelete
            // 
            this.buttonFolderDelete.Image = global::Core.Plugins.ProjectExplorer.Properties.Resources.DeleteHS;
            this.buttonFolderDelete.Name = "buttonFolderDelete";
            resources.ApplyResources(this.buttonFolderDelete, "buttonFolderDelete");
            this.buttonFolderDelete.Click += new System.EventHandler(this.deleteMenuItemClick);
            // 
            // buttonFolderRename
            // 
            this.buttonFolderRename.Image = global::Core.Plugins.ProjectExplorer.Properties.Resources.edit;
            this.buttonFolderRename.Name = "buttonFolderRename";
            resources.ApplyResources(this.buttonFolderRename, "buttonFolderRename");
            this.buttonFolderRename.Click += new System.EventHandler(this.buttonsRenameClick);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // buttonFolderExpandAll
            // 
            this.buttonFolderExpandAll.Image = global::Core.Plugins.ProjectExplorer.Properties.Resources.arrow_stop_270;
            this.buttonFolderExpandAll.Name = "buttonFolderExpandAll";
            resources.ApplyResources(this.buttonFolderExpandAll, "buttonFolderExpandAll");
            this.buttonFolderExpandAll.Click += new System.EventHandler(this.buttonFolderExpandAll_Click);
            // 
            // buttonFolderCollapseAll
            // 
            this.buttonFolderCollapseAll.Image = global::Core.Plugins.ProjectExplorer.Properties.Resources.arrow_stop_090;
            this.buttonFolderCollapseAll.Name = "buttonFolderCollapseAll";
            resources.ApplyResources(this.buttonFolderCollapseAll, "buttonFolderCollapseAll");
            this.buttonFolderCollapseAll.Click += new System.EventHandler(this.buttonFolderCollapseAll_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // buttonFolderProperties
            // 
            this.buttonFolderProperties.Image = global::Core.Plugins.ProjectExplorer.Properties.Resources.PropertiesHS;
            this.buttonFolderProperties.Name = "buttonFolderProperties";
            resources.ApplyResources(this.buttonFolderProperties, "buttonFolderProperties");
            this.buttonFolderProperties.Click += new System.EventHandler(this.buttonsPropertiesClick);
            // 
            // ProjectTreeView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ProjectTreeView";
            this.contextMenuProject.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuProject;
        private ClonableToolStripMenuItem buttonFolderAdd;
        private ClonableToolStripMenuItem buttonFolderAddNewItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private ClonableToolStripMenuItem buttonFolderImportFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private ClonableToolStripMenuItem buttonFolderProperties;
        private System.Windows.Forms.ToolStripMenuItem buttonFolderExportFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem buttonFolderRename;
        private System.Windows.Forms.ToolStripMenuItem buttonFolderDelete;
        private ClonableToolStripMenuItem buttonFolderExpandAll;
        private ClonableToolStripMenuItem buttonFolderCollapseAll;
    }
}