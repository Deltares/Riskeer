namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    partial class GrassCoverErosionInwardsScenariosView
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
            this.dataGridViewControl1 = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.SuspendLayout();
            // 
            // dataGridViewControl1
            // 
            this.dataGridViewControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl1.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewControl1.Name = "dataGridViewControl1";
            this.dataGridViewControl1.Size = new System.Drawing.Size(498, 451);
            this.dataGridViewControl1.TabIndex = 0;
            // 
            // GrassCoverErosionInwardsScenariosView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewControl1);
            this.Name = "GrassCoverErosionInwardsScenariosView";
            this.Size = new System.Drawing.Size(498, 451);
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl1;
    }
}
