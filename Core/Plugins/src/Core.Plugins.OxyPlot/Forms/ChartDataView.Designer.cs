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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartDataView));
            this.chart = new Core.Components.OxyPlot.Forms.BaseChart();
            this.SuspendLayout();
            // 
            // chart
            // 
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart.Location = new System.Drawing.Point(0, 0);
            this.chart.MinimumSize = new System.Drawing.Size(50, 75);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(150, 150);
            this.chart.TabIndex = 0;
            // 
            // ChartDataView
            // 
            this.Controls.Add(this.chart);
            this.MinimumSize = new System.Drawing.Size(0, 1);
            this.Name = "ChartDataView";
            this.ResumeLayout(false);

        }

        #endregion

        private BaseChart chart;
    }
}