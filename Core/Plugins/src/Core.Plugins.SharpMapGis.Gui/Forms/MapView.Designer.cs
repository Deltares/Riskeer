using Core.Common.Controls.Swf;
using Core.GIS.SharpMap.UI.Forms;

namespace Core.Plugins.SharpMapGis.Gui.Forms
{
    partial class MapView
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
            this.MapControl = new MapControl();
            this.SuspendLayout();
            // 
            // mapControl
            // 
            this.MapControl.AllowDrop = true;
            this.MapControl.BackColor = System.Drawing.Color.White;
            this.MapControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.MapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapControl.Location = new System.Drawing.Point(0, 0);
            this.MapControl.Name = "MapControl";
            this.MapControl.Size = new System.Drawing.Size(562, 380);
            this.MapControl.TabIndex = 0;
            this.MapControl.MouseEnter += new System.EventHandler(this.MapControlMouseEnter);
            this.MapControl.MouseLeave += new System.EventHandler(this.MapControlMouseLeave);
            this.MapControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MapControlMouseMove);
            // 
            // MapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MapControl);
            this.Name = "MapView";
            this.Size = new System.Drawing.Size(562, 388);
            this.ResumeLayout(false);

        }

        #endregion
    }
}