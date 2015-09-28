namespace DelftTools.TestUtils
{
    partial class ResourceMonitor
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
            this.label1 = new System.Windows.Forms.Label();
            this.labelAllocatedBitmaps = new System.Windows.Forms.Label();
            this.textBoxStackTrace = new System.Windows.Forms.TextBox();
            this.labelStackTrace = new System.Windows.Forms.Label();
            this.buttonPauseContinue = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of allocated bitmaps:";
            // 
            // labelAllocatedBitmaps
            // 
            this.labelAllocatedBitmaps.Location = new System.Drawing.Point(164, 13);
            this.labelAllocatedBitmaps.Name = "labelAllocatedBitmaps";
            this.labelAllocatedBitmaps.Size = new System.Drawing.Size(81, 13);
            this.labelAllocatedBitmaps.TabIndex = 1;
            this.labelAllocatedBitmaps.Text = "0";
            // 
            // textBoxStackTrace
            // 
            this.textBoxStackTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStackTrace.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxStackTrace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxStackTrace.Font = new System.Drawing.Font("Courier New", 9.163636F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxStackTrace.Location = new System.Drawing.Point(12, 54);
            this.textBoxStackTrace.Multiline = true;
            this.textBoxStackTrace.Name = "textBoxStackTrace";
            this.textBoxStackTrace.ReadOnly = true;
            this.textBoxStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxStackTrace.Size = new System.Drawing.Size(729, 438);
            this.textBoxStackTrace.TabIndex = 2;
            // 
            // labelStackTrace
            // 
            this.labelStackTrace.AutoSize = true;
            this.labelStackTrace.Location = new System.Drawing.Point(13, 33);
            this.labelStackTrace.Name = "labelStackTrace";
            this.labelStackTrace.Size = new System.Drawing.Size(65, 13);
            this.labelStackTrace.TabIndex = 3;
            this.labelStackTrace.Text = "Stack trace:";
            // 
            // buttonPauseContinue
            // 
            this.buttonPauseContinue.Location = new System.Drawing.Point(665, 13);
            this.buttonPauseContinue.Name = "buttonPauseContinue";
            this.buttonPauseContinue.Size = new System.Drawing.Size(76, 23);
            this.buttonPauseContinue.TabIndex = 4;
            this.buttonPauseContinue.Text = "Pause";
            this.buttonPauseContinue.UseVisualStyleBackColor = true;
            this.buttonPauseContinue.Click += new System.EventHandler(this.buttonPauseContinue_Click);
            // 
            // ResourceMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 504);
            this.Controls.Add(this.buttonPauseContinue);
            this.Controls.Add(this.labelStackTrace);
            this.Controls.Add(this.textBoxStackTrace);
            this.Controls.Add(this.labelAllocatedBitmaps);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Name = "ResourceMonitor";
            this.Text = "ResourceMonitor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelAllocatedBitmaps;
        private System.Windows.Forms.TextBox textBoxStackTrace;
        private System.Windows.Forms.Label labelStackTrace;
        private System.Windows.Forms.Button buttonPauseContinue;
    }
}