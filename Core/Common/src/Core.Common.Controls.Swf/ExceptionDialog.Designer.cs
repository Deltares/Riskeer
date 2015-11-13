using Core.Common.Controls.Swf.Properties;

namespace Core.Common.Controls.Swf
{
    partial class ExceptionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionDialog));
            this.buttonRestart = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonCopyTextToClipboard = new System.Windows.Forms.Button();
            this.exceptionTextBox = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonOpenLog = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonRestart
            // 
            resources.ApplyResources(this.buttonRestart, "buttonRestart");
            this.buttonRestart.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonRestart.Name = "buttonRestart";
            this.buttonRestart.UseVisualStyleBackColor = true;
            this.buttonRestart.Click += new System.EventHandler(this.buttonRestart_Click);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // buttonCopyTextToClipboard
            // 
            resources.ApplyResources(this.buttonCopyTextToClipboard, "buttonCopyTextToClipboard");
            this.buttonCopyTextToClipboard.Name = "buttonCopyTextToClipboard";
            this.buttonCopyTextToClipboard.UseVisualStyleBackColor = true;
            this.buttonCopyTextToClipboard.Click += new System.EventHandler(this.buttonCopyTextToClipboard_Click);
            // 
            // exceptionTextBox
            // 
            resources.ApplyResources(this.exceptionTextBox, "exceptionTextBox");
            this.exceptionTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.exceptionTextBox.ForeColor = System.Drawing.Color.Black;
            this.exceptionTextBox.Name = "exceptionTextBox";
            this.exceptionTextBox.ReadOnly = true;
            // 
            // richTextBox1
            // 
            resources.ApplyResources(this.richTextBox1, "richTextBox1");
            this.richTextBox1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            // 
            // buttonExit
            // 
            resources.ApplyResources(this.buttonExit, "buttonExit");
            this.buttonExit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonOpenLog
            // 
            resources.ApplyResources(this.buttonOpenLog, "buttonOpenLog");
            this.buttonOpenLog.Name = "buttonOpenLog";
            this.buttonOpenLog.UseVisualStyleBackColor = true;
            this.buttonOpenLog.Click += new System.EventHandler(this.buttonOpenLog_Click);
            // 
            // ExceptionDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.buttonOpenLog);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.exceptionTextBox);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.buttonCopyTextToClipboard);
            this.Controls.Add(this.buttonRestart);
            this.Name = "ExceptionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonRestart;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonCopyTextToClipboard;
        private System.Windows.Forms.RichTextBox exceptionTextBox;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonOpenLog;
    }
}