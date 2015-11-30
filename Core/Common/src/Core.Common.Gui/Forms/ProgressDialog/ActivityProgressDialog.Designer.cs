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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActivityProgressDialog));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelActivityCounter = new System.Windows.Forms.Label();
            this.labelActivityDescription = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 70);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(391, 23);
            this.progressBar.TabIndex = 0;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(415, 70);
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
            this.labelActivityCounter.Location = new System.Drawing.Point(13, 51);
            this.labelActivityCounter.Name = "labelActivityCounter";
            this.labelActivityCounter.Size = new System.Drawing.Size(68, 13);
            this.labelActivityCounter.TabIndex = 2;
            this.labelActivityCounter.Text = "Stap 1 van 1";
            // 
            // labelActivityDescription
            // 
            this.labelActivityDescription.AutoSize = true;
            this.labelActivityDescription.Location = new System.Drawing.Point(36, 20);
            this.labelActivityDescription.Name = "labelActivityDescription";
            this.labelActivityDescription.Size = new System.Drawing.Size(87, 13);
            this.labelActivityDescription.TabIndex = 3;
            this.labelActivityDescription.Text = "Uitvoeren \"taak\"";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Core.Common.Gui.Properties.Resources.Busy_indicator;
            this.pictureBox1.Location = new System.Drawing.Point(16, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // ActivityProgressDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 106);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelActivityDescription);
            this.Controls.Add(this.labelActivityCounter);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ActivityProgressDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Voortgang";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ActivityProgressDialogFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelActivityCounter;
        private System.Windows.Forms.Label labelActivityDescription;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}