namespace Core.Common.Controls.Swf.Csv
{
    partial class CsvToDataTableWizardPage
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
            this.csvToDataTableControl1 = new CsvToDataTableControl();
            this.SuspendLayout();
            // 
            // csvToDataTableControl1
            // 
            this.csvToDataTableControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.csvToDataTableControl1.Location = new System.Drawing.Point(0, 0);
            this.csvToDataTableControl1.Name = "csvToDataTableControl1";
            this.csvToDataTableControl1.Size = new System.Drawing.Size(469, 393);
            this.csvToDataTableControl1.TabIndex = 0;
            // 
            // CsvToDataTableWizardPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.csvToDataTableControl1);
            this.Name = "CsvToDataTableWizardPage";
            this.Size = new System.Drawing.Size(469, 393);
            this.ResumeLayout(false);

        }

        #endregion

        private CsvToDataTableControl csvToDataTableControl1;
    }
}
