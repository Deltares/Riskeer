namespace DelftTools.Controls.Swf.Csv
{
    partial class CsvToDataTableControl
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
            this.grpDelimeters = new System.Windows.Forms.GroupBox();
            this.chkEmptyLines = new System.Windows.Forms.CheckBox();
            this.chkHeader = new System.Windows.Forms.CheckBox();
            this.rbComma = new System.Windows.Forms.RadioButton();
            this.rbSemicolon = new System.Windows.Forms.RadioButton();
            this.rbSpace = new System.Windows.Forms.RadioButton();
            this.rbTab = new System.Windows.Forms.RadioButton();
            this.grpPreview = new System.Windows.Forms.GroupBox();
            this.dgvPreview = new System.Windows.Forms.DataGridView();
            this.grpDelimeters.SuspendLayout();
            this.grpPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // grpDelimeters
            // 
            this.grpDelimeters.Controls.Add(this.chkEmptyLines);
            this.grpDelimeters.Controls.Add(this.chkHeader);
            this.grpDelimeters.Controls.Add(this.rbComma);
            this.grpDelimeters.Controls.Add(this.rbSemicolon);
            this.grpDelimeters.Controls.Add(this.rbSpace);
            this.grpDelimeters.Controls.Add(this.rbTab);
            this.grpDelimeters.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpDelimeters.Location = new System.Drawing.Point(0, 0);
            this.grpDelimeters.Name = "grpDelimeters";
            this.grpDelimeters.Size = new System.Drawing.Size(454, 69);
            this.grpDelimeters.TabIndex = 5;
            this.grpDelimeters.TabStop = false;
            this.grpDelimeters.Text = "Delimeters";
            // 
            // chkEmptyLines
            // 
            this.chkEmptyLines.AutoSize = true;
            this.chkEmptyLines.Location = new System.Drawing.Point(302, 43);
            this.chkEmptyLines.Name = "chkEmptyLines";
            this.chkEmptyLines.Size = new System.Drawing.Size(111, 17);
            this.chkEmptyLines.TabIndex = 8;
            this.chkEmptyLines.Text = "Ignore empty lines";
            this.chkEmptyLines.UseVisualStyleBackColor = true;
            // 
            // chkHeader
            // 
            this.chkHeader.AutoSize = true;
            this.chkHeader.Location = new System.Drawing.Point(302, 21);
            this.chkHeader.Name = "chkHeader";
            this.chkHeader.Size = new System.Drawing.Size(134, 17);
            this.chkHeader.TabIndex = 7;
            this.chkHeader.Text = "Use first row as header";
            this.chkHeader.UseVisualStyleBackColor = true;
            // 
            // rbComma
            // 
            this.rbComma.AutoSize = true;
            this.rbComma.Checked = true;
            this.rbComma.Location = new System.Drawing.Point(76, 42);
            this.rbComma.Name = "rbComma";
            this.rbComma.Size = new System.Drawing.Size(60, 17);
            this.rbComma.TabIndex = 5;
            this.rbComma.TabStop = true;
            this.rbComma.Text = "Comma";
            this.rbComma.UseVisualStyleBackColor = true;
            // 
            // rbSemicolon
            // 
            this.rbSemicolon.AutoSize = true;
            this.rbSemicolon.Location = new System.Drawing.Point(76, 20);
            this.rbSemicolon.Name = "rbSemicolon";
            this.rbSemicolon.Size = new System.Drawing.Size(74, 17);
            this.rbSemicolon.TabIndex = 2;
            this.rbSemicolon.Text = "Semicolon";
            this.rbSemicolon.UseVisualStyleBackColor = true;
            // 
            // rbSpace
            // 
            this.rbSpace.AutoSize = true;
            this.rbSpace.Location = new System.Drawing.Point(6, 42);
            this.rbSpace.Name = "rbSpace";
            this.rbSpace.Size = new System.Drawing.Size(56, 17);
            this.rbSpace.TabIndex = 1;
            this.rbSpace.Text = "Space";
            this.rbSpace.UseVisualStyleBackColor = true;
            // 
            // rbTab
            // 
            this.rbTab.AutoSize = true;
            this.rbTab.Location = new System.Drawing.Point(6, 20);
            this.rbTab.Name = "rbTab";
            this.rbTab.Size = new System.Drawing.Size(44, 17);
            this.rbTab.TabIndex = 0;
            this.rbTab.Text = "Tab";
            this.rbTab.UseVisualStyleBackColor = true;
            // 
            // grpPreview
            // 
            this.grpPreview.Controls.Add(this.dgvPreview);
            this.grpPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPreview.Location = new System.Drawing.Point(0, 69);
            this.grpPreview.Name = "grpPreview";
            this.grpPreview.Size = new System.Drawing.Size(454, 296);
            this.grpPreview.TabIndex = 6;
            this.grpPreview.TabStop = false;
            this.grpPreview.Text = "Data preview";
            // 
            // dgvPreview
            // 
            this.dgvPreview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPreview.Location = new System.Drawing.Point(3, 16);
            this.dgvPreview.Name = "dgvPreview";
            this.dgvPreview.RowHeadersVisible = false;
            this.dgvPreview.Size = new System.Drawing.Size(448, 277);
            this.dgvPreview.TabIndex = 0;
            // 
            // CsvToDataTableControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpPreview);
            this.Controls.Add(this.grpDelimeters);
            this.Name = "CsvToDataTableControl";
            this.Size = new System.Drawing.Size(454, 365);
            this.grpDelimeters.ResumeLayout(false);
            this.grpDelimeters.PerformLayout();
            this.grpPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDelimeters;
        private System.Windows.Forms.RadioButton rbComma;
        private System.Windows.Forms.RadioButton rbSemicolon;
        private System.Windows.Forms.RadioButton rbSpace;
        private System.Windows.Forms.RadioButton rbTab;
        private System.Windows.Forms.GroupBox grpPreview;
        private System.Windows.Forms.DataGridView dgvPreview;
        private System.Windows.Forms.CheckBox chkHeader;
        private System.Windows.Forms.CheckBox chkEmptyLines;
    }
}
