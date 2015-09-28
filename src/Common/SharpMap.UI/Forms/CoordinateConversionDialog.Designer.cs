namespace SharpMap.UI.Forms
{
    partial class CoordinateConversionDialog
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
            this.rbAsIs = new System.Windows.Forms.RadioButton();
            this.rbDoTransformation = new System.Windows.Forms.RadioButton();
            this.txtFromCS = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtToCS = new System.Windows.Forms.TextBox();
            this.btnChooseFromCS = new System.Windows.Forms.Button();
            this.btnChooseToCS = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rbAsIs
            // 
            this.rbAsIs.AutoSize = true;
            this.rbAsIs.Checked = true;
            this.rbAsIs.Location = new System.Drawing.Point(12, 12);
            this.rbAsIs.Name = "rbAsIs";
            this.rbAsIs.Size = new System.Drawing.Size(190, 17);
            this.rbAsIs.TabIndex = 0;
            this.rbAsIs.TabStop = true;
            this.rbAsIs.Text = "Import without transformation (as-is)";
            this.rbAsIs.UseVisualStyleBackColor = true;
            // 
            // rbDoTransformation
            // 
            this.rbDoTransformation.AutoSize = true;
            this.rbDoTransformation.Location = new System.Drawing.Point(12, 35);
            this.rbDoTransformation.Name = "rbDoTransformation";
            this.rbDoTransformation.Size = new System.Drawing.Size(98, 17);
            this.rbDoTransformation.TabIndex = 1;
            this.rbDoTransformation.Text = "Transform from:";
            this.rbDoTransformation.UseVisualStyleBackColor = true;
            this.rbDoTransformation.CheckedChanged += new System.EventHandler(this.rbDoTransformation_CheckedChanged);
            // 
            // txtFromCS
            // 
            this.txtFromCS.Enabled = false;
            this.txtFromCS.Location = new System.Drawing.Point(116, 34);
            this.txtFromCS.Name = "txtFromCS";
            this.txtFromCS.ReadOnly = true;
            this.txtFromCS.Size = new System.Drawing.Size(200, 20);
            this.txtFromCS.TabIndex = 2;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(187, 88);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 25);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(268, 88);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(82, 25);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(91, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "to:";
            // 
            // txtToCS
            // 
            this.txtToCS.Enabled = false;
            this.txtToCS.Location = new System.Drawing.Point(116, 60);
            this.txtToCS.Name = "txtToCS";
            this.txtToCS.ReadOnly = true;
            this.txtToCS.Size = new System.Drawing.Size(200, 20);
            this.txtToCS.TabIndex = 2;
            // 
            // btnChooseFromCS
            // 
            this.btnChooseFromCS.Enabled = false;
            this.btnChooseFromCS.Location = new System.Drawing.Point(316, 34);
            this.btnChooseFromCS.Name = "btnChooseFromCS";
            this.btnChooseFromCS.Size = new System.Drawing.Size(34, 20);
            this.btnChooseFromCS.TabIndex = 5;
            this.btnChooseFromCS.Text = "...";
            this.btnChooseFromCS.UseVisualStyleBackColor = true;
            this.btnChooseFromCS.Click += new System.EventHandler(this.btnChooseFromCS_Click);
            // 
            // btnChooseToCS
            // 
            this.btnChooseToCS.Enabled = false;
            this.btnChooseToCS.Location = new System.Drawing.Point(316, 60);
            this.btnChooseToCS.Name = "btnChooseToCS";
            this.btnChooseToCS.Size = new System.Drawing.Size(34, 20);
            this.btnChooseToCS.TabIndex = 6;
            this.btnChooseToCS.Text = "...";
            this.btnChooseToCS.UseVisualStyleBackColor = true;
            this.btnChooseToCS.Click += new System.EventHandler(this.btnChooseToCS_Click);
            // 
            // CoordinateConversionDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(362, 125);
            this.Controls.Add(this.btnChooseToCS);
            this.Controls.Add(this.btnChooseFromCS);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtToCS);
            this.Controls.Add(this.txtFromCS);
            this.Controls.Add(this.rbDoTransformation);
            this.Controls.Add(this.rbAsIs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CoordinateConversionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Apply coordinate transformation on data?";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbAsIs;
        private System.Windows.Forms.RadioButton rbDoTransformation;
        private System.Windows.Forms.TextBox txtFromCS;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtToCS;
        private System.Windows.Forms.Button btnChooseFromCS;
        private System.Windows.Forms.Button btnChooseToCS;
    }
}