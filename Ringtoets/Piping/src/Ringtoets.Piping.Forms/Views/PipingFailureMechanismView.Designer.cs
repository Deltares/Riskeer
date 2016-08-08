namespace Ringtoets.Piping.Forms.Views
{
    partial class PipingFailureMechanismView
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
            this.mapControl = new Core.Components.DotSpatial.Forms.MapControl();
            this.SuspendLayout();
            // 
            // MapView
            //
            this.mapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapControl.Location = new System.Drawing.Point(0, 0);
            this.mapControl.Name = "mapControl";
            this.mapControl.Size = new System.Drawing.Size(150, 150);
            this.mapControl.TabIndex = 0;
            this.mapControl.Text = "mapControl1";
            // 
            // PipingFailureMechanismView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapControl);
            this.Name = "PipingFailureMechanismView";
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Components.DotSpatial.Forms.MapControl mapControl;
    }
}
