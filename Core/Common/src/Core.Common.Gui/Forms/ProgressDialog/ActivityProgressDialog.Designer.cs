namespace Core.Common.Gui.Forms.ProgressDialog
{
    partial class ActivityProgressDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelActivityCounter = new System.Windows.Forms.Label();
            this.labelActivityDescription = new System.Windows.Forms.Label();
            this.pictureBoxActivityDescription = new System.Windows.Forms.PictureBox();
            this.pictureBoxActivityProgressText = new System.Windows.Forms.PictureBox();
            this.labelActivityProgressText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxActivityDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxActivityProgressText)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 79);
            this.progressBar.MarqueeAnimationSpeed = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(391, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 0;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(416, 79);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Annuleren";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancelClick);
            // 
            // labelActivityCounter
            // 
            this.labelActivityCounter.AutoSize = true;
            this.labelActivityCounter.Location = new System.Drawing.Point(13, 60);
            this.labelActivityCounter.Name = "labelActivityCounter";
            this.labelActivityCounter.Size = new System.Drawing.Size(68, 13);
            this.labelActivityCounter.TabIndex = 2;
            this.labelActivityCounter.Text = "Stap 1 van 1";
            // 
            // labelActivityDescription
            // 
            this.labelActivityDescription.AutoSize = true;
            this.labelActivityDescription.Location = new System.Drawing.Point(36, 11);
            this.labelActivityDescription.Name = "labelActivityDescription";
            this.labelActivityDescription.Size = new System.Drawing.Size(77, 13);
            this.labelActivityDescription.TabIndex = 3;
            this.labelActivityDescription.Text = "Uitvoeren taak";
            // 
            // pictureBoxActivityDescription
            // 
            this.pictureBoxActivityDescription.Image = global::Core.Common.Gui.Properties.Resources.Busy_indicator;
            this.pictureBoxActivityDescription.Location = new System.Drawing.Point(16, 9);
            this.pictureBoxActivityDescription.Name = "pictureBoxActivityDescription";
            this.pictureBoxActivityDescription.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxActivityDescription.TabIndex = 4;
            this.pictureBoxActivityDescription.TabStop = false;
            // 
            // pictureBoxActivityProgressText
            // 
            this.pictureBoxActivityProgressText.Image = global::Core.Common.Gui.Properties.Resources.arrow_000_medium;
            this.pictureBoxActivityProgressText.Location = new System.Drawing.Point(39, 29);
            this.pictureBoxActivityProgressText.Name = "pictureBoxActivityProgressText";
            this.pictureBoxActivityProgressText.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxActivityProgressText.TabIndex = 5;
            this.pictureBoxActivityProgressText.TabStop = false;
            // 
            // labelActivityProgressText
            // 
            this.labelActivityProgressText.AutoSize = true;
            this.labelActivityProgressText.Location = new System.Drawing.Point(61, 31);
            this.labelActivityProgressText.Name = "labelActivityProgressText";
            this.labelActivityProgressText.Size = new System.Drawing.Size(128, 13);
            this.labelActivityProgressText.TabIndex = 6;
            this.labelActivityProgressText.Text = "Stap 1 van 10  |  Subtaak";
            // 
            // ActivityProgressDialog
            // 
            this.ClientSize = new System.Drawing.Size(504, 112);
            this.Controls.Add(this.labelActivityProgressText);
            this.Controls.Add(this.pictureBoxActivityProgressText);
            this.Controls.Add(this.pictureBoxActivityDescription);
            this.Controls.Add(this.labelActivityDescription);
            this.Controls.Add(this.labelActivityCounter);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ActivityProgressDialog";
            this.Text = "Voortgang";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ActivityProgressDialogFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxActivityDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxActivityProgressText)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelActivityCounter;
        private System.Windows.Forms.Label labelActivityDescription;
        private System.Windows.Forms.PictureBox pictureBoxActivityDescription;
        private System.Windows.Forms.PictureBox pictureBoxActivityProgressText;
        private System.Windows.Forms.Label labelActivityProgressText;
    }
}