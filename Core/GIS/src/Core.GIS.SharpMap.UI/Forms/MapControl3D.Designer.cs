namespace SharpMap.UI.Forms
{
    partial class MapControl3D
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
            this.SuspendLayout();
            // 
            // MapControl3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "MapControl3D";
            this.Size = new System.Drawing.Size(877, 600);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MapControl3D_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MapControl3D_MouseDown);
            this.Resize += new System.EventHandler(this.MapControl3D_Resize);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MapControl3D_MouseUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MapControl3D_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
