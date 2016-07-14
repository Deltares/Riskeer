namespace Core.Plugins.DotSpatial.Legend
{
    partial class MapLegendView
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
            this.treeViewControl = new Core.Common.Controls.TreeView.TreeViewControl();
            this.SuspendLayout();
            // 
            // treeViewControl
            // 
            this.treeViewControl.Data = null;
            this.treeViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewControl.Location = new System.Drawing.Point(0, 0);
            this.treeViewControl.Name = "treeViewControl";
            this.treeViewControl.Size = new System.Drawing.Size(150, 150);
            this.treeViewControl.TabIndex = 0;
            // 
            // MapLegendView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewControl);
            this.Name = "MapLegendView";
            this.ResumeLayout(false);

        }

        #endregion

        private Common.Controls.TreeView.TreeViewControl treeViewControl;
    }
}
