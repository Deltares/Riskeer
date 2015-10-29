namespace Core.Common.Controls.Swf.Table
{
    partial class TableViewDataToolBar
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.pasteAssistBtn = new System.Windows.Forms.ToolStripButton();
            this.csvImportBtn = new System.Windows.Forms.ToolStripButton();
            this.csvExportBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pasteAssistBtn,
            this.csvImportBtn,
            this.csvExportBtn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(321, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // pasteAssistBtn
            // 
            this.pasteAssistBtn.Image = global::Core.Common.Controls.Swf.Properties.Resources.PasteHS;
            this.pasteAssistBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteAssistBtn.Name = "pasteAssistBtn";
            this.pasteAssistBtn.Size = new System.Drawing.Size(118, 22);
            this.pasteAssistBtn.Text = "Clipboard import";
            this.pasteAssistBtn.Click += new System.EventHandler(this.pasteAssistBtn_Click);
            // 
            // csvImportBtn
            // 
            this.csvImportBtn.Image = global::Core.Common.Controls.Swf.Properties.Resources.csv_import;
            this.csvImportBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.csvImportBtn.Name = "csvImportBtn";
            this.csvImportBtn.Size = new System.Drawing.Size(85, 22);
            this.csvImportBtn.Text = "Csv import";
            this.csvImportBtn.Click += new System.EventHandler(this.csvImportBtn_Click);
            // 
            // csvExportBtn
            // 
            this.csvExportBtn.Image = global::Core.Common.Controls.Swf.Properties.Resources.csv_export;
            this.csvExportBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.csvExportBtn.Name = "csvExportBtn";
            this.csvExportBtn.Size = new System.Drawing.Size(82, 22);
            this.csvExportBtn.Text = "Csv export";
            this.csvExportBtn.Click += new System.EventHandler(this.csvExportBtn_Click);
            // 
            // TableViewDataToolBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Name = "TableViewDataToolBar";
            this.Size = new System.Drawing.Size(321, 22);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton pasteAssistBtn;
        private System.Windows.Forms.ToolStripButton csvImportBtn;
        private System.Windows.Forms.ToolStripButton csvExportBtn;
    }
}
