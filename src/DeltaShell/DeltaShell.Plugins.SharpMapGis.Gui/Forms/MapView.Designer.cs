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
            this.MapControl = new SharpMap.UI.Forms.MapControl();
            this.Splitter = new DelftTools.Controls.Swf.CollapsibleSplitter();
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
            // collapsibleSplitter1
            // 
            this.Splitter.AnimationDelay = 20;
            this.Splitter.AnimationStep = 20;
            this.Splitter.BorderStyle3D = System.Windows.Forms.Border3DStyle.Flat;
            this.Splitter.ControlToHide = null;
            this.Splitter.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.Splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Splitter.ExpandParentForm = false;
            this.Splitter.Location = new System.Drawing.Point(0, 380);
            this.Splitter.Name = "Splitter";
            this.Splitter.TabIndex = 3;
            this.Splitter.TabStop = false;
            this.Splitter.UseAnimations = false;
            this.Splitter.VisualStyle = DelftTools.Controls.Swf.VisualStyles.Lines;
            // 
            // MapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MapControl);
            this.Controls.Add(this.Splitter);
            this.Name = "MapView";
            this.Size = new System.Drawing.Size(562, 388);
            this.ResumeLayout(false);

        }

        #endregion
    }
}