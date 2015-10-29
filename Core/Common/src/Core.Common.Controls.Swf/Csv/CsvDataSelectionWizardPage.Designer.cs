namespace Core.Common.Controls.Swf.Csv
{
    partial class CsvDataSelectionWizardPage
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
            this.csvDataSelectionControl1 = new CsvDataSelectionControl();
            this.SuspendLayout();
            // 
            // csvDataSelectionControl1
            // 
            this.csvDataSelectionControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.csvDataSelectionControl1.Location = new System.Drawing.Point(0, 0);
            this.csvDataSelectionControl1.Name = "csvDataSelectionControl1";
            this.csvDataSelectionControl1.Size = new System.Drawing.Size(403, 375);
            this.csvDataSelectionControl1.TabIndex = 0;
            // 
            // CsvDataSelectionWizardPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.csvDataSelectionControl1);
            this.Name = "CsvDataSelectionWizardPage";
            this.Size = new System.Drawing.Size(403, 375);
            this.ResumeLayout(false);

        }

        #endregion

        private CsvDataSelectionControl csvDataSelectionControl1;
    }
}
