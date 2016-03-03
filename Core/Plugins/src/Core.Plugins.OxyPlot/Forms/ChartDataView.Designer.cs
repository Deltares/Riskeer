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
            this.chartControl = new Core.Components.OxyPlot.Forms.ChartControl();
            this.SuspendLayout();
            // 
            // chart
            // 
            this.chartControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl.Location = new System.Drawing.Point(0, 0);
            this.chartControl.MinimumSize = new System.Drawing.Size(50, 75);
            this.chartControl.Name = "chartControl";
            this.chartControl.Size = new System.Drawing.Size(150, 150);
            this.chartControl.TabIndex = 0;
            // 
            // ChartDataView
            // 
            this.Controls.Add(this.chartControl);
            this.MinimumSize = new System.Drawing.Size(0, 1);
            this.Name = "ChartDataView";
            this.ResumeLayout(false);

        }

        #endregion

        private ChartControl chartControl;
    }
}