namespace Core.Common.Controls.Swf.Csv
{
    partial class CsvDataSelectionControl
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
            this.groupColumnSelection = new System.Windows.Forms.GroupBox();
            this.groupFiltering = new System.Windows.Forms.GroupBox();
            this.btApplyFilter = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.filterColumnCombobox = new System.Windows.Forms.ComboBox();
            this.checkBoxUseFilter = new System.Windows.Forms.CheckBox();
            this.dataGridBefore = new System.Windows.Forms.DataGridView();
            this.groupCultureInfo = new System.Windows.Forms.GroupBox();
            this.dataGridAfter = new System.Windows.Forms.DataGridView();
            this.panelBefore = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panelAfter = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.groupFiltering.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridBefore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridAfter)).BeginInit();
            this.panelBefore.SuspendLayout();
            this.panelAfter.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupColumnSelection
            // 
            this.groupColumnSelection.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupColumnSelection.Location = new System.Drawing.Point(0, 18);
            this.groupColumnSelection.Margin = new System.Windows.Forms.Padding(8);
            this.groupColumnSelection.Name = "groupColumnSelection";
            this.groupColumnSelection.Padding = new System.Windows.Forms.Padding(8);
            this.groupColumnSelection.Size = new System.Drawing.Size(592, 20);
            this.groupColumnSelection.TabIndex = 1;
            this.groupColumnSelection.TabStop = false;
            this.groupColumnSelection.Text = "Column selection";
            // 
            // groupFiltering
            // 
            this.groupFiltering.Controls.Add(this.btApplyFilter);
            this.groupFiltering.Controls.Add(this.label1);
            this.groupFiltering.Controls.Add(this.textBoxFilter);
            this.groupFiltering.Controls.Add(this.filterColumnCombobox);
            this.groupFiltering.Controls.Add(this.checkBoxUseFilter);
            this.groupFiltering.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupFiltering.Location = new System.Drawing.Point(0, 38);
            this.groupFiltering.Margin = new System.Windows.Forms.Padding(8);
            this.groupFiltering.Name = "groupFiltering";
            this.groupFiltering.Padding = new System.Windows.Forms.Padding(8);
            this.groupFiltering.Size = new System.Drawing.Size(592, 56);
            this.groupFiltering.TabIndex = 2;
            this.groupFiltering.TabStop = false;
            this.groupFiltering.Text = "Filtering";
            // 
            // btApplyFilter
            // 
            this.btApplyFilter.Location = new System.Drawing.Point(450, 19);
            this.btApplyFilter.Name = "btApplyFilter";
            this.btApplyFilter.Size = new System.Drawing.Size(45, 20);
            this.btApplyFilter.TabIndex = 4;
            this.btApplyFilter.Text = "Apply";
            this.btApplyFilter.UseVisualStyleBackColor = true;
            this.btApplyFilter.Click += new System.EventHandler(this.btApplyFilter_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(260, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Filter value";
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Enabled = false;
            this.textBoxFilter.Location = new System.Drawing.Point(324, 20);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(120, 20);
            this.textBoxFilter.TabIndex = 2;
            // 
            // filterColumnCombobox
            // 
            this.filterColumnCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterColumnCombobox.Enabled = false;
            this.filterColumnCombobox.Location = new System.Drawing.Point(93, 22);
            this.filterColumnCombobox.Name = "filterColumnCombobox";
            this.filterColumnCombobox.Size = new System.Drawing.Size(161, 21);
            this.filterColumnCombobox.TabIndex = 1;
            // 
            // checkBoxUseFilter
            // 
            this.checkBoxUseFilter.AutoSize = true;
            this.checkBoxUseFilter.Location = new System.Drawing.Point(11, 24);
            this.checkBoxUseFilter.Name = "checkBoxUseFilter";
            this.checkBoxUseFilter.Size = new System.Drawing.Size(67, 17);
            this.checkBoxUseFilter.TabIndex = 0;
            this.checkBoxUseFilter.Text = "Use filter";
            this.checkBoxUseFilter.UseVisualStyleBackColor = true;
            this.checkBoxUseFilter.CheckedChanged += new System.EventHandler(this.checkBoxUseFilter_CheckedChanged);
            // 
            // dataGridBefore
            // 
            this.dataGridBefore.AllowUserToAddRows = false;
            this.dataGridBefore.AllowUserToDeleteRows = false;
            this.dataGridBefore.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridBefore.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridBefore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridBefore.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridBefore.Location = new System.Drawing.Point(0, 13);
            this.dataGridBefore.Margin = new System.Windows.Forms.Padding(8);
            this.dataGridBefore.Name = "dataGridBefore";
            this.dataGridBefore.ReadOnly = true;
            this.dataGridBefore.Size = new System.Drawing.Size(592, 172);
            this.dataGridBefore.TabIndex = 3;
            // 
            // groupCultureInfo
            // 
            this.groupCultureInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupCultureInfo.Location = new System.Drawing.Point(0, 0);
            this.groupCultureInfo.Margin = new System.Windows.Forms.Padding(8);
            this.groupCultureInfo.Name = "groupCultureInfo";
            this.groupCultureInfo.Size = new System.Drawing.Size(592, 18);
            this.groupCultureInfo.TabIndex = 0;
            this.groupCultureInfo.TabStop = false;
            this.groupCultureInfo.Text = "Culture info";
            // 
            // dataGridAfter
            // 
            this.dataGridAfter.AllowUserToAddRows = false;
            this.dataGridAfter.AllowUserToDeleteRows = false;
            this.dataGridAfter.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridAfter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridAfter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridAfter.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridAfter.Location = new System.Drawing.Point(0, 13);
            this.dataGridAfter.Margin = new System.Windows.Forms.Padding(8);
            this.dataGridAfter.Name = "dataGridAfter";
            this.dataGridAfter.ReadOnly = true;
            this.dataGridAfter.Size = new System.Drawing.Size(592, 149);
            this.dataGridAfter.TabIndex = 4;
            // 
            // panelBefore
            // 
            this.panelBefore.Controls.Add(this.dataGridBefore);
            this.panelBefore.Controls.Add(this.label2);
            this.panelBefore.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBefore.Location = new System.Drawing.Point(0, 94);
            this.panelBefore.Name = "panelBefore";
            this.panelBefore.Size = new System.Drawing.Size(592, 185);
            this.panelBefore.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Input preview";
            // 
            // panelAfter
            // 
            this.panelAfter.Controls.Add(this.dataGridAfter);
            this.panelAfter.Controls.Add(this.label3);
            this.panelAfter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAfter.Location = new System.Drawing.Point(0, 279);
            this.panelAfter.Name = "panelAfter";
            this.panelAfter.Size = new System.Drawing.Size(592, 162);
            this.panelAfter.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Result preview";
            // 
            // CsvDataSelectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelAfter);
            this.Controls.Add(this.panelBefore);
            this.Controls.Add(this.groupFiltering);
            this.Controls.Add(this.groupColumnSelection);
            this.Controls.Add(this.groupCultureInfo);
            this.Name = "CsvDataSelectionControl";
            this.Size = new System.Drawing.Size(592, 441);
            this.groupFiltering.ResumeLayout(false);
            this.groupFiltering.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridBefore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridAfter)).EndInit();
            this.panelBefore.ResumeLayout(false);
            this.panelBefore.PerformLayout();
            this.panelAfter.ResumeLayout(false);
            this.panelAfter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupColumnSelection;
        private System.Windows.Forms.GroupBox groupFiltering;
        private System.Windows.Forms.DataGridView dataGridBefore;
        private System.Windows.Forms.GroupBox groupCultureInfo;
        private System.Windows.Forms.CheckBox checkBoxUseFilter;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.ComboBox filterColumnCombobox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridAfter;
        private System.Windows.Forms.Panel panelBefore;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelAfter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btApplyFilter;

    }
}
