namespace Demo.Ringtoets.Views
{
    partial class MapDataView
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
            this.mapControl = new Core.Components.DotSpatial.Forms.MapControl();
            this.SuspendLayout();
            // 
            // mapControl1
            // 
            this.mapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapControl.Location = new System.Drawing.Point(0, 0);
            this.mapControl.Name = "mapControl1";
            this.mapControl.Size = new System.Drawing.Size(150, 150);
            this.mapControl.TabIndex = 0;
            this.mapControl.Text = "mapControl1";
            // 
            // MapDataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapControl);
            this.Name = "MapDataView";
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Components.DotSpatial.Forms.MapControl mapControl;
    }
}
