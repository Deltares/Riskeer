using SharpMap.UI.Forms;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms
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
            this.mapControl = new SharpMap.UI.Forms.MapControl();
            this.collapsibleSplitter1 = new DelftTools.Controls.Swf.CollapsibleSplitter();
            this.SuspendLayout();
            // 
            // mapControl
            // 
            this.mapControl.AllowDrop = true;
            this.mapControl.BackColor = System.Drawing.Color.White;
            this.mapControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.mapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapControl.Location = new System.Drawing.Point(0, 0);
            this.mapControl.Name = "mapControl";
            this.mapControl.Size = new System.Drawing.Size(562, 380);
            this.mapControl.TabIndex = 0;
            this.mapControl.MouseEnter += new System.EventHandler(this.MapControlMouseEnter);
            this.mapControl.MouseLeave += new System.EventHandler(this.MapControlMouseLeave);
            this.mapControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MapControlMouseMove);
            // 
            // collapsibleSplitter1
            // 
            this.collapsibleSplitter1.AnimationDelay = 20;
            this.collapsibleSplitter1.AnimationStep = 20;
            this.collapsibleSplitter1.BorderStyle3D = System.Windows.Forms.Border3DStyle.Flat;
            this.collapsibleSplitter1.ControlToHide = null;
            this.collapsibleSplitter1.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.collapsibleSplitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.collapsibleSplitter1.ExpandParentForm = false;
            this.collapsibleSplitter1.Location = new System.Drawing.Point(0, 380);
            this.collapsibleSplitter1.Name = "collapsibleSplitter1";
            this.collapsibleSplitter1.TabIndex = 3;
            this.collapsibleSplitter1.TabStop = false;
            this.collapsibleSplitter1.UseAnimations = false;
            this.collapsibleSplitter1.VisualStyle = DelftTools.Controls.Swf.VisualStyles.Lines;
            // 
            // MapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapControl);
            this.Controls.Add(this.collapsibleSplitter1);
            this.Name = "MapView";
            this.Size = new System.Drawing.Size(562, 388);
            this.ResumeLayout(false);

        }

        #endregion

        private MapControl mapControl;
        private DelftTools.Controls.Swf.CollapsibleSplitter collapsibleSplitter1;
    }
}