namespace DelftTools.Controls.Swf.Table.Filtering
{
    partial class FilterTableColumnControl
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelColumnName = new System.Windows.Forms.Label();
            this.panelEditor = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOk = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonClearFilter = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelColumnName
            // 
            this.labelColumnName.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.labelColumnName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelColumnName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelColumnName.Location = new System.Drawing.Point(0, 0);
            this.labelColumnName.Name = "labelColumnName";
            this.labelColumnName.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.labelColumnName.Size = new System.Drawing.Size(118, 20);
            this.labelColumnName.TabIndex = 1;
            this.labelColumnName.Text = "Column";
            this.labelColumnName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelEditor
            // 
            this.panelEditor.AutoSize = true;
            this.panelEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEditor.Location = new System.Drawing.Point(0, 20);
            this.panelEditor.Name = "panelEditor";
            this.panelEditor.Size = new System.Drawing.Size(118, 13);
            this.panelEditor.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonCancel,
            this.toolStripButtonOk,
            this.toolStripButtonClearFilter});
            this.toolStrip1.Location = new System.Drawing.Point(0, 33);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(118, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonCancel
            // 
            this.toolStripButtonCancel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonCancel.Image = global::DelftTools.Controls.Swf.Properties.Resources.cross;
            this.toolStripButtonCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCancel.Name = "toolStripButtonCancel";
            this.toolStripButtonCancel.Size = new System.Drawing.Size(63, 22);
            this.toolStripButtonCancel.Text = "Cancel";
            this.toolStripButtonCancel.Click += new System.EventHandler(this.toolStripButtonCancel_Click);
            // 
            // toolStripButtonOk
            // 
            this.toolStripButtonOk.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonOk.Image = global::DelftTools.Controls.Swf.Properties.Resources.tick;
            this.toolStripButtonOk.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOk.Name = "toolStripButtonOk";
            this.toolStripButtonOk.Size = new System.Drawing.Size(42, 20);
            this.toolStripButtonOk.Text = "Ok";
            this.toolStripButtonOk.Click += new System.EventHandler(this.toolStripButtonOk_Click);
            // 
            // toolStripButtonClearFilter
            // 
            this.toolStripButtonClearFilter.Image = global::DelftTools.Controls.Swf.Properties.Resources.funneminus;
            this.toolStripButtonClearFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClearFilter.Name = "toolStripButtonClearFilter";
            this.toolStripButtonClearFilter.Size = new System.Drawing.Size(81, 20);
            this.toolStripButtonClearFilter.Text = "Clear filter";
            this.toolStripButtonClearFilter.Click += new System.EventHandler(this.toolStripButtonClearFilter_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelColumnName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(118, 20);
            this.panel1.TabIndex = 3;
            // 
            // FilterTableColumnControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelEditor);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(120, 60);
            this.Name = "FilterTableColumnControl";
            this.Size = new System.Drawing.Size(118, 58);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelColumnName;
        private System.Windows.Forms.Panel panelEditor;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancel;
        private System.Windows.Forms.ToolStripButton toolStripButtonOk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton toolStripButtonClearFilter;
    }
}