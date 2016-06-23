using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    partial class GrassCoverErosionInwardsInputView
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
            this.chartControl = new Core.Components.OxyPlot.Forms.ChartControl();
            this.SuspendLayout();
            // 
            // chartControl
            // 
            this.chartControl.BottomAxisTitle = "L";
            this.chartControl.ChartTitle = null;
            this.chartControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl.LeftAxisTitle = "Hoogte";
            this.chartControl.Location = new System.Drawing.Point(0, 0);
            this.chartControl.MinimumSize = new System.Drawing.Size(50, 75);
            this.chartControl.Name = "chartControl";
            this.chartControl.Size = new System.Drawing.Size(150, 150);
            this.chartControl.TabIndex = 0;
            this.chartControl.Text = "chartControl";
            this.chartControl.BottomAxisTitle = RingtoetsCommonFormsResources.InputView_Distance_DisplayName;
            this.chartControl.LeftAxisTitle = RingtoetsCommonFormsResources.InputView_Height_DisplayName;
            // 
            // GrassCoverErosionInwardsInputView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chartControl);
            this.Name = "GrassCoverErosionInwardsInputView";
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Components.OxyPlot.Forms.ChartControl chartControl;
    }
}
