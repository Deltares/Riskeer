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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValidationView));
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.manualRefreshPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.manualRefreshButton = new System.Windows.Forms.Button();
            this.validationReportControl = new Core.Common.Gui.Swf.Validation.ValidationReportControl();
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
            resources.ApplyResources(this.manualRefreshPanel, "manualRefreshPanel");
            this.manualRefreshPanel.Name = "manualRefreshPanel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // manualRefreshButton
            // 
            this.manualRefreshButton.BackColor = System.Drawing.SystemColors.Info;
            resources.ApplyResources(this.manualRefreshButton, "manualRefreshButton");
            this.manualRefreshButton.Name = "manualRefreshButton";
            this.manualRefreshButton.UseVisualStyleBackColor = false;
            this.manualRefreshButton.Click += new System.EventHandler(this.manualRefreshButton_Click);
            // 
            // validationReportControl
            // 
            this.validationReportControl.Data = null;
            resources.ApplyResources(this.validationReportControl, "validationReportControl");
            this.validationReportControl.Name = "validationReportControl";
            this.validationReportControl.ViewInfo = null;
            // 
            // ValidationView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.validationReportControl);
            this.Controls.Add(this.manualRefreshPanel);
            this.Name = "ValidationView";
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
