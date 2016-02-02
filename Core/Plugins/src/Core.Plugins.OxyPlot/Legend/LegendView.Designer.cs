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
            this.treeViewControl = new TreeViewControl();
            this.SuspendLayout();
            // 
            // treeViewControl
            // 
            resources.ApplyResources(this.treeViewControl, "treeViewControl");
            this.treeViewControl.Name = "treeViewControl";
            // 
            // LegendView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewControl);
            this.Name = "LegendView";
            this.ResumeLayout(false);

        }

        #endregion

        private TreeViewControl treeViewControl;
    }
}