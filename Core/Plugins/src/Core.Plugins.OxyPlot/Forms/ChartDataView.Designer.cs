using Core.Components.OxyPlot.Forms;

namespace Core.Plugins.OxyPlot.Forms
{
    partial class ChartDataView
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

        private void InitializeComponent()
        {
            this.Chart = new Core.Components.OxyPlot.Forms.BaseChart();
            this.SuspendLayout();
            // 
            // chart
            // 
            this.Chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Chart.Location = new System.Drawing.Point(0, 0);
            this.Chart.MinimumSize = new System.Drawing.Size(50, 75);
            this.Chart.Name = "Chart";
            this.Chart.Size = new System.Drawing.Size(150, 150);
            this.Chart.TabIndex = 0;
            // 
            // ChartDataView
            // 
            this.Controls.Add(this.Chart);
            this.MinimumSize = new System.Drawing.Size(0, 1);
            this.Name = "ChartDataView";
            this.ResumeLayout(false);

        }

        #endregion
    }
}