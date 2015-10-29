namespace Core.Common.Gui.Swf.Validation
{
    partial class ValidationView
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
            this.components = new System.ComponentModel.Container();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.manualRefreshPanel = new System.Windows.Forms.Panel();
            this.manualRefreshButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.validationReportControl = new ValidationReportControl();
            this.manualRefreshPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // refreshTimer
            // 
            this.refreshTimer.Enabled = true;
            this.refreshTimer.Interval = 750;
            this.refreshTimer.Tick += new System.EventHandler(this.RefreshTimerTick);
            // 
            // manualRefreshPanel
            // 
            this.manualRefreshPanel.BackColor = System.Drawing.SystemColors.Info;
            this.manualRefreshPanel.Controls.Add(this.label1);
            this.manualRefreshPanel.Controls.Add(this.manualRefreshButton);
            this.manualRefreshPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.manualRefreshPanel.Location = new System.Drawing.Point(0, 0);
            this.manualRefreshPanel.Name = "manualRefreshPanel";
            this.manualRefreshPanel.Size = new System.Drawing.Size(805, 32);
            this.manualRefreshPanel.TabIndex = 3;
            // 
            // manualRefreshButton
            // 
            this.manualRefreshButton.BackColor = System.Drawing.SystemColors.Info;
            this.manualRefreshButton.Location = new System.Drawing.Point(3, 3);
            this.manualRefreshButton.Name = "manualRefreshButton";
            this.manualRefreshButton.Size = new System.Drawing.Size(72, 26);
            this.manualRefreshButton.TabIndex = 0;
            this.manualRefreshButton.Text = "Refresh...";
            this.manualRefreshButton.UseVisualStyleBackColor = false;
            this.manualRefreshButton.Click += new System.EventHandler(this.manualRefreshButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(81, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(363, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Auto refresh has been turned off due to model size; please refresh manually.";
            // 
            // validationReportControl
            // 
            this.validationReportControl.Data = null;
            this.validationReportControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.validationReportControl.Image = null;
            this.validationReportControl.Location = new System.Drawing.Point(0, 32);
            this.validationReportControl.Name = "validationReportControl";
            this.validationReportControl.Size = new System.Drawing.Size(805, 482);
            this.validationReportControl.TabIndex = 2;
            this.validationReportControl.ViewInfo = null;
            // 
            // ValidationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.validationReportControl);
            this.Controls.Add(this.manualRefreshPanel);
            this.Name = "ValidationView";
            this.Size = new System.Drawing.Size(805, 514);
            this.manualRefreshPanel.ResumeLayout(false);
            this.manualRefreshPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ValidationReportControl validationReportControl;
        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.Panel manualRefreshPanel;
        private System.Windows.Forms.Button manualRefreshButton;
        private System.Windows.Forms.Label label1;
    }
}
