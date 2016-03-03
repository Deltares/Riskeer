namespace Ringtoets.Piping.Forms.Views
{
    partial class PipingFailureMechanismView
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
            this.MapView = new Core.Components.DotSpatial.Forms.BaseMap();
            this.SuspendLayout();
            // 
            // MapView
            // 
            this.MapView.Data = null;
            this.MapView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapView.Location = new System.Drawing.Point(0, 0);
            this.MapView.Name = "MapView";
            this.MapView.Size = new System.Drawing.Size(150, 150);
            this.MapView.TabIndex = 0;
            this.MapView.Text = "baseMap1";
            // 
            // PipingFailureMechanismView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MapView);
            this.Name = "PipingFailureMechanismView";
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Components.DotSpatial.Forms.BaseMap MapView;
    }
}
